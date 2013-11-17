using System;
using System.Linq;
using System.Threading.Tasks;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Diagnostic;
using PhoneGuitarTab.Core.Services;
using PhoneGuitarTab.Data;

namespace PhoneGuitarTab.UI.Infrastructure
{
    /// <summary>
    /// Synchronizes tab collection with cloud storage
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

        private TraceCategory _traceCategory;

        private const string CloudRootPath = "PhoneGuitarTab";

        public TabSyncService()
        {
            _traceCategory = new TraceCategory("TabSyncService");
        }

        public async void Synchronize()
        {
            Trace.Info(_traceCategory, "start synchronization");

            await IsoToCloudSync();

            await CloudToIsoSync();

            Trace.Info(_traceCategory, "synchronize complete");
            OnComplete();
        }

        /// <summary>
        /// Tracks sync progress
        /// </summary>
        public event EventHandler<int> Progress;

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

        /// <summary>
        /// Synchronization: Iso -> Cloud
        /// </summary>
        private async Task IsoToCloudSync()
        {
            foreach (var group in DataService.Groups)
            {
                Trace.Info(_traceCategory, String.Format("synchronize group {0}", group.Name));
                foreach (var tab in group.Tabs)
                {
                    Trace.Info(_traceCategory, String.Format("synchronize tab {0}", tab.Path));
                    //await CloudService.CreateDirectory(string.Format("{0}/{1}/", CloudRootPath, group.Name));
                    await  CloudService.SynchronizeFile(tab.Path, GetCloudNameFromIso(tab));
                }
            }
        }

        /// <summary>
        /// Synchronization: Cloud -> Iso
        /// </summary>
        private async Task CloudToIsoSync()
        {
            var groupNames = await CloudService.GetDirectoryNames(CloudRootPath);
            foreach (var groupName in groupNames)
            {
                Trace.Info(_traceCategory, String.Format("synchronize group {0}", groupName));
               
                var group = DataService.GetOrCreateGroupByName(groupName);
                var localFileNamesMapped = group.Tabs.Select(GetCloudNameFromIso);
                var cloudFileNames = await CloudService.GetFileNames(String.Format("{0}/{1}", CloudRootPath, groupName));

                // all these tabs should be downloaded from skydrive to iso
                var newTabs = cloudFileNames.Intersect(localFileNamesMapped);
                foreach (var newTab in newTabs)
                {
                    Trace.Info(_traceCategory, String.Format("synchronize tab {0}", newTab));
                    var localPath = TabFileStorage.CreateTabFilePath();
                    await CloudService.DownloadFile(localPath, newTab);
                    DataService.InsertTab(GetTabFromName(newTab, localPath, group));
                }
            }
        }


        private Tab GetTabFromName(string cloudName, string localPath, Group group)
        {          
            var tabType = cloudName.Contains(".gp") ?
                DataService.TabTypes.Single(t => t.Name == Strings.GuitarPro) : 
                DataService.TabTypes.Single(t => t.Name == "tab");

            var name = cloudName;
            int fileExtPos = name.LastIndexOf(".");
            if (fileExtPos >= 0)
                name = name.Substring(0, fileExtPos);

            var tab = new Tab()
            {
                Group = group,
                TabType = tabType,
                Name = name,
                Path = localPath
            };
            return tab;
        }

        private string GetCloudNameFromIso(Tab tab)
        {
            // NOTE Hardcoded file extension
            return String.Format("{0}/{1}/{2}_{3}.gp5", CloudRootPath, tab.Group.Name, tab.Name, tab.Id);
        }
    }
}
