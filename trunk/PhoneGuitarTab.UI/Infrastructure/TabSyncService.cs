using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Diagnostic;
using PhoneGuitarTab.Core.Services;
using PhoneGuitarTab.Data;
using Group = PhoneGuitarTab.Data.Group;

namespace PhoneGuitarTab.UI.Infrastructure
{
    /// <summary>
    /// Synchronizes tab collection with cloud storage.
    /// </summary>
    public class TabSyncService
    {
        [Dependency]
        private ICloudService CloudService { get; set; }
        
        [Dependency]
        private IDataContextService DataService { get; set; }

        [Dependency]
        private TabFileStorage TabFileStorage { get; set; }

        [Dependency]
        private ITrace Trace { get; set; }

        private const int HalfOfProgress = 50;

        public bool DownloadSyncFiles { get; set; }

        private Regex _syncSignatureRegex;

        private TraceCategory _traceCategory;

        private int _progressValue;
        private int ProgressValue
        {
            get
            {
                return _progressValue;
            }
            set
            {
                _progressValue = value;
                OnProgress(_progressValue);
            }
        }

        private int _uploaded;

        public int Uploaded
        {
            get
            {
                return _uploaded;
            }
            set
            {
                _uploaded = value;
                OnProgress(_progressValue);
            }
        }

        private int _downloaded;
        public int Downloaded
        {
            get
            {
                return _downloaded;
            }
            set
            {
                _downloaded = value;
                OnProgress(_progressValue);
            }
        }



        private const string CloudRootPath = "PhoneGuitarTab";

        public TabSyncService()
        {
            _syncSignatureRegex = new Regex(@".*_sync_\d+\.", RegexOptions.Compiled);
            _traceCategory = new TraceCategory("TabSyncService");

            DownloadSyncFiles = false;
        }

        public async void Synchronize(bool downloadAll = false)
        {
            try
            {
                Uploaded = 0;
                Downloaded = 0;

                Trace.Info(_traceCategory, "sign in");
                await CloudService.SignIn();

                Trace.Info(_traceCategory, "start synchronization");

                ProgressValue = 0;
                await IsoToCloudSync();

                ProgressValue = HalfOfProgress;
                await CloudToIsoSync(downloadAll);

                ProgressValue = HalfOfProgress*2;
                Trace.Info(_traceCategory, "synchronize complete");
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error occurred during synchronization: {0}", ex.Message));
            }
            finally
            {
                // NOTE seems to be workaround: need to clear underlying cache to prevent attempts to 
                // upload files several times in case of serial synchronize button clicks
                // Defenitely need to replace implementation of CloudService!
                CloudService.Release();
                ProgressValue = 0;
                OnComplete();
            }
        }

        /// <summary>
        /// Tracks sync progress
        /// </summary>
        public event EventHandler<int> Progress;

        /// <summary>
        /// Provides information about current action
        /// </summary>
        public event EventHandler<IProgressMessage> ProgressMessage;

        /// <summary>
        /// Raised when sync is completed
        /// </summary>
        public event EventHandler Complete;

        protected virtual void OnComplete()
        {
            EventHandler handler = Complete;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnProgress(int e)
        {
            EventHandler<int> handler = Progress;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnProgressMessage(IProgressMessage progressMessage)
        {
            EventHandler<IProgressMessage> handler = ProgressMessage;
            if (handler != null) handler(this, progressMessage);
        }

        /// <summary>
        /// Synchronization: Iso -> Cloud
        /// </summary>
        private async Task IsoToCloudSync()
        {
            var groupsCount = DataService.Groups.Count();
            var progressIncrement = HalfOfProgress / (groupsCount == 0 ? 1 : groupsCount); // just to prevent arithmetic error
            foreach (var group in DataService.Groups)
            {
                Trace.Info(_traceCategory, String.Format("synchronize group {0}", group.Name));
                foreach (var tab in group.Tabs)
                {
                    Trace.Info(_traceCategory, String.Format("synchronize tab {0}", tab.Path));
                    var cloudName = GetCloudName(tab);
                    if (!await CloudService.FileExists(cloudName))
                    {
                        OnProgressMessage(new UploadProgressMessage(group.Name, tab.Name));
                        await CloudService.UploadFile(tab.Path, cloudName);
                        Uploaded++;
                    }

                }
                ProgressValue += progressIncrement;
            }
        }

        /// <summary>
        /// Synchronization: Cloud -> Iso
        /// </summary>
        private async Task CloudToIsoSync(bool downloadAll = false)
        {
            var groupNames = (await CloudService.GetDirectoryNames(CloudRootPath)).ToList();
            var groupsCount = groupNames.Count();
            var progressIncrement = HalfOfProgress / (groupsCount == 0 ? 1 : groupsCount);
            foreach (var groupName in groupNames)
            {
                Trace.Info(_traceCategory, String.Format("synchronize group {0}", groupName));
               
                var group = DataService.GetOrCreateGroupByName(groupName);
                var localFileNamesMapped = group.Tabs.Select(GetCloudName);
                var cloudFileNames = await CloudService.GetFileNames(String.Format("{0}/{1}", CloudRootPath, groupName));

                // get tabs which aren't present in iso
                var newTabs = cloudFileNames.Except(localFileNamesMapped).ToList();

                // these tabs was synchronized, but deleted then.
                // NOTE it's possible that user reinstalled application, but had already synchronized files
                // just handle this situation using special option
                var deletedTabs = newTabs.Where(t => !downloadAll & IsMappedPath(t)).ToList();
                
                //if (!DownloadSyncFiles)
                newTabs = newTabs.Except(deletedTabs).ToList();

                // filter cloud synchronized files without _sync_
                newTabs = newTabs.Where( name => !IsCloudName(name, group)).ToList();

                // all these tabs should be downloaded from skydrive to iso
                foreach (var newTab in newTabs)
                {
                    Trace.Info(_traceCategory, String.Format("synchronize tab {0}", newTab));
                    var localName = TabFileStorage.CreateTabFilePath();
                    OnProgressMessage(new DownloadProgressMessage(groupName, newTab));
                    await CloudService.DownloadFile(localName, string.Format("{0}/{1}/{2}", CloudRootPath, groupName, newTab));
                    DataService.InsertTab(GetTab(newTab, localName, group));
                    Downloaded++;
                }

                ProgressValue += progressIncrement;
            }
            DataService.SubmitChanges();
        }

        private bool IsCloudName(string name, Group @group)
        {
            return @group.Tabs.Any(tab => tab.CloudName == name);
        }


        private bool IsMappedPath(string name)
        {
            return _syncSignatureRegex.IsMatch(name);
        }

        private Tab GetTab(string cloudName, string localName, Group group)
        {
            var name = cloudName;
            int fileExtPos = name.LastIndexOf(".");
            if (fileExtPos >= 0)
                name = name.Substring(0, fileExtPos);

            // TODO remove _sync_ suffix if present (executed only for downloadAll option)
            if (IsMappedPath(name))
            {
                var index = name.LastIndexOf("_sync_");
                name = name.Substring(0, index);
            }

            var tab = new Tab()
            {
                Group = group,
                TabType = GetTabTypeByName(cloudName),
                Name = name,
                Path = localName,
                CloudName = cloudName
            };
            return tab;
        }

        private string GetCloudName(Tab tab)
        {
            // get tablature extension
            var extension = GetExtensionByType(tab.TabType);

            return string.IsNullOrEmpty(tab.CloudName)?
                // NOTE this signature should prevent name collisions
                // and acts as marker of already synchronized tabs
                String.Format("{0}/{1}/{2}_sync_{3}.{4}", CloudRootPath, tab.Group.Name, tab.Name, tab.Id, extension) : 
                String.Format("{0}/{1}/{2}", CloudRootPath, tab.Group.Name, tab.CloudName);
        }

        private string GetExtensionByType(TabType type)
        {
            switch (type.Name)
            {
                case Strings.GuitarPro:
                    return "gp5";
                case Strings.MusicXml:
                    return "xml";
                default:
                    return "txt";
            }
        }

        private TabType GetTabTypeByName(string name)
        {
            if (name.Contains(".gp"))
                return DataService.TabTypes.Single(t => t.Name == Strings.GuitarPro);

            if(name.Contains(".xml"))
                return DataService.TabTypes.Single(t => t.Name == Strings.MusicXml);

            return DataService.TabTypes.Single(t => t.Name == "tab");
        }

        #region Progress info

        // NOTE maybe seems like overengineering but will be useful if we want to log all other actions (e.g., getting list of tabs)

        public interface IProgressMessage
        {
            string GetMessage();
        }

        // TODO support info about size of tab
        public abstract class TabProgressMessage : IProgressMessage
        {
            public string Group { get; set; }
            public string Tab { get; set; }
            public int Size { get; set; }

            protected TabProgressMessage(string group, string tab, int size)
            {
                Group = group;
                Tab = tab;
                Size = size;
            }

            protected abstract string GetAction();

            public string GetMessage()
            {
                return string.Format("{0} {1} - {2}", GetAction(), Group, Tab);
            }
        }

        public class DownloadProgressMessage : TabProgressMessage
        {
            public DownloadProgressMessage(string group, string tab) : base(group, tab, 0) { }

            protected override string GetAction()
            {
                return "Downloading";
            }
        }

        public class UploadProgressMessage : TabProgressMessage
        {
            public UploadProgressMessage(string group, string tab) : base(group, tab, 0) { }

            protected override string GetAction()
            {
                return "Uploading";
            }
        }

        #endregion
    }
}
