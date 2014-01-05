
using System.Windows;
using System.Windows.Input;

using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Views.Commands;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class SynchronizeViewModel : PhoneGuitarTab.Core.Views.ViewModel
    {
        public TabSyncService SyncService { get; set; }

        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChanged("Progress");
            }
        }

        private bool _isSyncNotRunning;
        public bool IsSyncNotRunning
        {
            get { return _isSyncNotRunning; }
            set
            {
                _isSyncNotRunning = value;
                RaisePropertyChanged("IsSyncNotRunning");
            }
        }

        private int _downloadedTabs;
        public int DownloadedTabs
        {
            get { return _downloadedTabs; }
            set
            {
                _downloadedTabs = value;
                RaisePropertyChanged("DownloadedTabs");
            }
        }

        private bool _downloadAll;
        public bool DownloadAll
        {
            get { return _downloadAll; }
            set
            {
                _downloadAll = value;
                RaisePropertyChanged("DownloadAll");
            }
        } 

        private int _uploadedTabs;
        public int UploadedTabs
        {
            get { return _uploadedTabs; }
            set
            {
                _uploadedTabs = value;
                RaisePropertyChanged("UploadedTabs");
            }
        } 

        [Dependency]
        public SynchronizeViewModel(TabSyncService syncService, MessageHub hub)
        {
            SyncService = syncService;
            SyncService.Progress += (sender, i) =>
            {
                Progress = i;
                DownloadedTabs = SyncService.Downloaded;
                UploadedTabs = SyncService.Uploaded;
            };
            SyncService.Complete += (o, e) =>
            {
                IsSyncNotRunning = true;
                hub.RaiseTabsDownloaded();
                hub.RaiseTabsRefreshed();
            };
            IsSyncNotRunning = true;
        }

        public ICommand LaunchSync
        {
            get
            {
                return new ExecuteCommand(() =>
                {
                    IsSyncNotRunning = false;
                    SyncService.Synchronize(DownloadAll);
                });
            }
        }
    }
}
