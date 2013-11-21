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
    /// </summary>
    public class SkyDriveService : ICloudService
    {

        #region Data Members
        private string _folderId; //folder's id in skydrive
        private readonly string _clientId; //skydrive application client id
        private readonly string _folderName;

        [Dependency]
        private IFileSystemService FileSystemService { get; set; }

        //LiveConnect members
        private LiveConnectClient _liveClient;
        private LiveAuthClient _liveAuth;
        private LiveLoginResult _liveResult;
        private LiveConnectSession _liveSession; // NOTE refactor class to get existing session from outside if it's necessary
        private readonly string[] _requiredScopes;
        #endregion

        #region Ctor's
        /// <summary>
        /// Initializes SkyDriveService
        /// </summary>
        [Dependency]
        public SkyDriveService(SkyDriveAppContext context)
        {
            this._clientId = context.ClientId;
            _folderName = context.AppFolder;

            if (context.Scopes == null)
            {
                //setting scopes by default
                _requiredScopes = new string[] { "wl.basic", "wl.skydrive", "wl.offline_access", "wl.signin", "wl.skydrive_update" };
            }  
        }
        #endregion

        #region Private Methods


        private async Task SignIn()
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

            await GetSkyDriveFolder();
        }

        /// <summary>
        /// Gets the id for a existing skydrive folder or creates a new one and gets an id for it
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetSkyDriveFolder(bool createIfNonExist = true)
        {
            if (_liveClient == null)
                await SignIn();

            _folderId = await GetFolderId("me/skydrive/files/", _folderName);
           
            //the folder hasn't been found, so let's create a new one
            if (_folderId == null && createIfNonExist)
                _folderId = await CreateSkydriveFolder(_folderName);

            return _folderId;
        }

        private async Task<string> GetFolderId(string parent, string path)
        {
            LiveOperationResult result = await _liveClient.GetAsync(parent);

            //if (result.Result.ContainsKey("id"))
            //    return result.Result["id"].ToString();

            List<object> data = (List<object>)result.Result["data"];
            foreach (IDictionary<string, object> content in data)
            {
                if (content["name"].ToString() == path)
                {
                    return content["id"].ToString();
                }
            }
            return null;
        }

        public async Task<OperationStatus> CreateDirectory(string relativePath)
        {
            try
            {
                if (_liveClient == null)
                    await SignIn();
                await GetFolderIdAndFileName(relativePath);
                //var fold = await CreateSkydriveFolder(relativePath);
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

        public Task<IEnumerable<string>> GetFileNames(string relativePath)
        {
            return ListContent(c => c["type"].ToString() != "folder");
        }

        private async  Task<IEnumerable<string>> ListContent(Func<IDictionary<string,object>, bool> func)
        {
            if (_liveClient == null)
                await SignIn();

            LiveOperationResult fileResult = await _liveClient.GetAsync(_folderId + "/files");
            List<object> fileData = (List<object>)fileResult.Result["data"];
            List<string> data = new List<string>();
            foreach (IDictionary<string, object> content in fileData)
            {
                if (func(content))
                    data.Add(content["name"].ToString());
            }
            return data;
        }

        /// <summary>
        /// Creates a new skydrive folder and returns its id
        /// </summary>
        /// <returns></returns>
        private async Task<string> CreateSkydriveFolder(string relativePath)
        {
            return await CreateSkydriveFolder("me/skydrive", relativePath);
        }

        private async Task<string> CreateSkydriveFolder(string rootPath, string path)
        {
            string id = null;
            try
            {
                var folderData = new Dictionary<string, object>();
                folderData.Add("name", path);
                LiveOperationResult operationResult = await _liveClient.PostAsync(rootPath, folderData);
                dynamic result = operationResult.Result;
                id = string.Format("{0}", result.id);
            }
            catch(Exception ex)
            {
                //...something is wrong...
            }
            return id;
        }

        /// <summary>
        /// moves downloaded file into app's local storage
        /// </summary>
        /// <param name="fileName">Name of a downloaded file</param>
        /// <param name="fileID">File's ID in SkyDrive</param>
        /// <returns></returns>
        private async Task MoveToLocalStorage(string fileName, string fileID)
        {
            try
            {
                LiveOperationResult res = await _liveClient.BackgroundDownloadAsync(fileID + "/content",
                            new Uri("/shared/transfers/" + fileName, UriKind.Relative));

                if (FileSystemService.FileExists(fileName))
                    FileSystemService.DeleteFile(fileName);
                FileSystemService.MoveFile("/shared/transfers/" + fileName, fileName);
                
            }
            catch (Exception ex)
            {
                //...something is wrong...
                MessageBox.Show("The error occured while uploading the file! " + ex.Message);
            }

        }
        #endregion

        #region

        /// <summary>
        /// Build recursivly path and returns folder id and filename
        /// </summary>
        public async Task<Tuple<string,string>> GetFolderIdAndFileName(string path)
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
                    id = await GetFolderId(string.Format("{0}/files?filter=folders", _folderId), directory) ??
                        await CreateSkydriveFolder(_folderId, directory);
                    p = p.Substring(index, p.Length - index);
                }
                else
                {
                    break;
                }
            }

            return new Tuple<string, string>(id, p);
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Upload a file from local storage to a SkyDrive's folder
        /// </summary>
        /// <param name="fileName">Name of a file in LocalStorage</param>
        /// <returns></returns>
        public async Task<OperationStatus> UploadFile(string localPath, string cloudPath)
        {
            using (var fileStream = FileSystemService.OpenFile(localPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                try
                {
                    if (_liveClient == null)
                        await GetSkyDriveFolder();

                    var tuple = await GetFolderIdAndFileName(cloudPath);

                    LiveOperationResult res = await _liveClient.UploadAsync(tuple.Item1,
                                                tuple.Item2,
                                                fileStream,
                                                OverwriteOption.Overwrite
                                                );
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
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<OperationStatus> DownloadFile(string localPath, string cloudPath)
        {
            string fileID = null;

            if (_liveClient == null)
                await GetSkyDriveFolder();

            //looking for the file
            LiveOperationResult fileResult = await _liveClient.GetAsync(_folderId + "/files");
            List<object> fileData = (List<object>)fileResult.Result["data"];
            foreach (IDictionary<string, object> content in fileData)
            {
                if (content["name"].ToString() == cloudPath)
                {
                    //The file has been found!
                    fileID = content["id"].ToString();
                    await MoveToLocalStorage(localPath, fileID);
                    return OperationStatus.Completed;
                }
            }
            return OperationStatus.Failed;
        }

        public Task<OperationStatus> DeleteFile(string cloudPath)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// Compares files in the local storage and SkyDrive folder
        /// Gets the newest file and updates the older one
        /// </summary>
        /// <returns></returns>
        public async Task<OperationStatus> SynchronizeFile(string localPath, string cloudPath)
        {
            if (_liveClient == null)
                await GetSkyDriveFolder();

            string fileID = null;

            //looking for the file in skydrive folder
            LiveOperationResult fileResult = await _liveClient.GetAsync(_folderId + "/files");
            List<object> fileData = (List<object>)fileResult.Result["data"];
            foreach (IDictionary<string, object> content in fileData)
            {
                 if (content["name"].ToString() == localPath)
                 {
                     //The file has been found!
                     fileID = content["id"].ToString();
                     DateTime fileUpdatedTime = new DateTime();
                     if (DateTime.TryParse(content["updated_time"].ToString(), out fileUpdatedTime))
                     {
                             if (FileSystemService.FileExists(localPath))
                             {
                                 //the file already exists in the local storage. let's compare dates
                                 if (fileUpdatedTime > FileSystemService.GetLastWriteTime(localPath).DateTime)
                                 {
                                     await MoveToLocalStorage(localPath, fileID);
                                 }
                                 else if (fileUpdatedTime < FileSystemService.GetLastWriteTime(localPath).DateTime)
                                 {
                                     //local file is newer than skydrive file
                                     //updating skydrive file
                                     await UploadFile(localPath, cloudPath);
                                 }                                 
                             }
                             else
                             {
                                 //the file hasn't been found in LocalStorage
                                 //so we're downloading it and move into app's localstorage
                                 await MoveToLocalStorage(cloudPath, fileID);
                             }
                             return OperationStatus.Completed;
                          
                      }
                }                
            }
            //upload the file to skydrive if file hasn't been found there 
            if (fileID == null)
            {

                if (FileSystemService.FileExists(localPath))
                    {
                        await UploadFile(localPath, cloudPath);
                        return OperationStatus.Completed;
                    }
                               
            }

            return OperationStatus.Failed;

        }

        #endregion       

        public class SkyDriveAppContext
        {
            public string ClientId { get; set; }
            public string AppFolder { get; set; }
            public string[] Scopes { get; set; }
        }

    }
}
