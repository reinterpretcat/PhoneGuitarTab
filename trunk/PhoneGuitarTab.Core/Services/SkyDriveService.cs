using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Live;
using PhoneGuitarTab.Core.Dependencies;

namespace PhoneGuitarTab.Core.Services
{
    /// <summary>
    /// SkyDrive file manager
    /// TODO refactor this class or switch to usage of better livesdk wrapper
    /// </summary>
    public class SkyDriveService : ICloudService
    {

        #region Data Members
        private string _folderId; //folder's id in skydrive
        private readonly string _clientId; //skydrive application client id
        private readonly string _folderName;
        private readonly string[] _requiredScopes;
        
        [Dependency]
        private IFileSystemService FileSystemService { get; set; }

        //LiveConnect members
        private LiveConnectClient _liveClient;
        private LiveAuthClient _liveAuth;
        private LiveLoginResult _liveResult;
        private LiveConnectSession _liveSession; // NOTE refactor class to get existing session from outside if it's necessary

        private SkyDriveService.Cache _cache = new SkyDriveService.Cache();

        #endregion

        #region Ctor's
        /// <summary>
        /// Initializes SkyDriveService
        /// </summary>
        [Dependency]
        public SkyDriveService(SkyDriveAppContext context)
        {
            _clientId = context.ClientId;
            _folderName = context.AppFolder;

            if (context.Scopes == null)
            {
                //setting scopes by default
                _requiredScopes = new string[] { "wl.basic", "wl.skydrive", "wl.offline_access", "wl.signin", "wl.skydrive_update" };
            }  
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the id for a existing skydrive folder or creates a new one and gets an id for it
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetSkyDriveFolder(bool createIfNonExist = true)
        {
            if (_liveClient == null)
                throw new InvalidOperationException("Unable to initialize liveSession");

            _folderId = await GetFolderId("me/skydrive", _folderName);
           
            //the folder hasn't been found, so let's create a new one
            if (_folderId == null && createIfNonExist)
                _folderId = await CreateSkydriveFolder(_folderName);

            return _folderId;
        }

        private async Task<string> GetFolderId(string parent, string path)
        {
            // Filter only directories
            var data = await GetFolderFiles(parent);

            return (from IDictionary<string, object> content in data 
                    where content["name"].ToString() == path 
                    select content["id"].ToString()).FirstOrDefault();
        }

        public async Task<OperationStatus> CreateDirectory(string relativePath)
        {
            try
            {
                if (_liveClient == null) await SignIn();
                await GetFolderIdAndFileName(relativePath);
            }
            catch
            {
                return OperationStatus.Failed;
            }

            return OperationStatus.Completed;
        }

        public Task<IEnumerable<string>> GetDirectoryNames(string relativePath)
        {
            return ListContent(c => c["type"].ToString() == "folder");
        }

        public async Task<IEnumerable<string>> GetFileNames(string relativePath)
        { 
            var folderId = (await GetFolderIdAndFileName(relativePath + "/")).Item1;
            return await ListContent(c => c["type"].ToString() != "folder", folderId);
        }

        private async  Task<IEnumerable<string>> ListContent(Func<IDictionary<string,object>, bool> func, string folderId = null)
        {
            if (_liveClient == null) await SignIn();

            if (folderId == null) folderId = _folderId;

            var folderData = await GetFolderFiles(folderId);

            return (from IDictionary<string, object> content in folderData 
                    where func(content) 
                    select content["name"].ToString()).ToList();
        }

        /// <summary>
        /// Creates a new skydrive folder and returns its id
        /// </summary>
        private async Task<string> CreateSkydriveFolder(string relativePath)
        {
            return await CreateSkydriveFolder("me/skydrive", relativePath);
        }

        private async Task<string> CreateSkydriveFolder(string rootPath, string path)
        {
            try
            {
                var folderData = new Dictionary<string, object>();
                folderData.Add("name", path);
                LiveOperationResult operationResult = await _liveClient.PostAsync(rootPath, folderData);
                
                dynamic result = operationResult.Result;
                InvalidateDirectoryInCache(result.id);

                return string.Format("{0}", result.id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void InvalidateDirectoryInCache(string directory)
        {
            _cache.Remove(directory + "/files");
        }

        /// <summary>
        /// moves downloaded file into app's local storage
        /// </summary>
        /// <param name="fileName">Name of a downloaded file</param>
        /// <param name="fileID">File's ID in SkyDrive</param>
        private async Task MoveToLocalStorage(string fileName, string fileID)
        {
            try
            {
                var tempName = Guid.NewGuid().ToString();
                LiveOperationResult res = await _liveClient.BackgroundDownloadAsync(fileID + "/content",
                            new Uri("/shared/transfers/" + tempName, UriKind.Relative));

                if (FileSystemService.FileExists(fileName))
                    FileSystemService.DeleteFile(fileName);
                FileSystemService.MoveFile("/shared/transfers/" + tempName, fileName);
                
            }
            catch (Exception ex)
            {
                //...something is wrong...
                MessageBox.Show("The error occured while uploading the file! " + ex.Message);
            }

        }
        #endregion


        #region Public Methods

        public async Task SignIn()
        {
            // you should call this from UI thread
            if (_liveSession == null)
            {
                _liveAuth = new LiveAuthClient(_clientId);
                _liveResult = await _liveAuth.InitializeAsync(_requiredScopes);

                if (_liveResult.Status != LiveConnectSessionStatus.Connected)
                {
                    _liveResult = await _liveAuth.LoginAsync(_requiredScopes);
                }

                _liveSession = _liveResult.Session;
                _liveClient = new LiveConnectClient(_liveSession);
            }

            if (_folderId == null) await GetSkyDriveFolder();

        }


        /// <summary>
        /// Upload a file from local storage to a SkyDrive's folder
        /// </summary>
        public async Task<OperationStatus> UploadFile(string localPath, string cloudPath)
        {
            using (var fileStream = FileSystemService.OpenFile(localPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                try
                {
                    if (_liveClient == null) await SignIn();

                    var tuple = await GetFolderIdAndFileName(cloudPath);

                    LiveOperationResult res = await _liveClient.UploadAsync(tuple.Item1,
                                                tuple.Item2,
                                                fileStream,
                                                OverwriteOption.Overwrite);
                    return OperationStatus.Completed;
                }
                catch(Exception ex)
                {
                    return OperationStatus.Failed;
                }
            }
        }

        /// <summary>
        /// Downloads a file from SkyDrive and saves it in LocalStorage
        /// If the file exists it will be overwritten
        /// </summary>
        public async Task<OperationStatus> DownloadFile(string localPath, string cloudPath)
        {
            string fileID = null;

            if (_liveClient == null)
                await SignIn();

            var fileInfo = await GetFolderIdAndFileName(cloudPath);           
            var folderData = await GetFolderFiles(fileInfo.Item1);
        
            foreach (IDictionary<string, object> content in folderData)
            {
                if (content["name"].ToString() == fileInfo.Item2)
                {
                    //The file has been found!
                    fileID = content["id"].ToString();
                    await MoveToLocalStorage(localPath, fileID);
                    return OperationStatus.Completed;
                }
            }
            return OperationStatus.Failed;
        }
 
        public async Task<bool> FileExists(string fileName)
        {
            if (_liveClient == null)
                await SignIn();

            var fileInfo = await GetFolderIdAndFileName(fileName, false);

            if (fileInfo == null) return false;

            var folderData = await GetFolderFiles(fileInfo.Item1);

            return folderData.Cast<IDictionary<string, object>>()
                .Any(content => content["name"].ToString() == fileInfo.Item2);
        }

        public void Release()
        {
            _cache.Clear();
        }

        #endregion

        /// <summary>
        ///  Gets folder content
        /// </summary>
        private async Task<List<object>> GetFolderFiles(string directory)
        {
            var path = directory + "/files";
            return await _cache.GetOrExecuteAsync(path, async () =>
            {
                LiveOperationResult fr = await _liveClient.GetAsync(path);
                return (List<object>)fr.Result["data"];
            });
        }


       /* /// <summary>
        ///  Gets folder content
        /// </summary>
        private async Task<List<object>> GetChildFolders(string directory)
        {
            var path = directory + "/files?filter=folders";
            return await _cache.GetOrExecuteAsync(path, async () =>
            {
                LiveOperationResult fr = await _liveClient.GetAsync(path);
                return (List<object>)fr.Result["data"];
            });
        }*/


        /// <summary>
        /// Build recursivly path and returns folder id and filename
        /// </summary>
        private async Task<Tuple<string, string>> GetFolderIdAndFileName(string path, bool createFolderIfNonExist = true)
        {
            var p = path.Substring(_folderName.Length, path.Length - _folderName.Length);
            var id = _folderId;
            int index = 0;
            var directory = "";
            while (p.StartsWith("/"))
            {
                p = p.Substring(1, p.Length - 1);
                index = p.IndexOf("/");
                if (index >= 0)
                {
                    directory = p.Substring(0, index);
                    
                    id = await GetFolderId(_folderId, directory) ??
                        (createFolderIfNonExist ? await CreateSkydriveFolder(_folderId, directory): null);

                    // NOTE this situation is definetly possible, if createFolderIfNonExist is set to false
                    // so return null as result
                    if (id == null)
                        return null;
                    
                    p = p.Substring(index, p.Length - index);
                }
                else
                {
                    break;
                }
            }

            return new Tuple<string, string>(id, p);
        }

        #region Nested classes

        private class Cache
        {
            private int _cleanCounter;

            private Dictionary<string, Tuple<DateTime, object>> _cacheMap = new Dictionary<string, Tuple<DateTime, object>>();

            public async Task<T> GetOrExecuteAsync<T>(string key, Func<Task<T>> func) where T : class
            {
                var @value = Get<T>(key);
                if (@value != null) 
                    return @value;

                @value = await func();
                Set(key, @value);
                return @value;
            }
           
            public T Get<T>(string key) where T: class
            {
                CheckAndClear();
                Tuple<DateTime, object> tuple  = null;
                if (!_cacheMap.TryGetValue(key, out tuple)) return null;

                if ((DateTime.Now - tuple.Item1).Seconds < 5*60)
                {
                    return (T) tuple.Item2;
                }

                _cacheMap.Remove(key);

                return null;
            }

            public void Set(string key, object @value)
            {
                _cacheMap.Add(key, new Tuple<DateTime, object>(DateTime.Now, @value));
            }

            public void Remove(string key)
            {
                if (_cacheMap.ContainsKey(key))
                    _cacheMap.Remove(key);
            }

            private void CheckAndClear()
            {
                // clear cache to prevent memory leaks
                if (_cleanCounter++ == 100)
                {
                    _cacheMap = new Dictionary<string, Tuple<DateTime, object>>();
                }
            }

            public void Clear()
            {
                _cacheMap.Clear();
            }
        }

        public class SkyDriveAppContext
        {
            public string ClientId { get; set; }
            public string AppFolder { get; set; }
            public string[] Scopes { get; set; }
        }

        #endregion
    }
}
