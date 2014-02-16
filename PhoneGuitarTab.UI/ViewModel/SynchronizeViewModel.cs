
using System.Windows;
using System.Windows.Input;
using System;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Views.Commands;
using PhoneGuitarTab.UI.Infrastructure;
using Microsoft.Phone.Shell;
using System.Runtime.InteropServices;
namespace PhoneGuitarTab.UI.ViewModel
{
    public class SynchronizeViewModel : PhoneGuitarTab.Core.Views.ViewModel
    {
        public TabSyncService SyncService { get; set; }
        public ProgressIndicator ProgressIndicator { get; set; }
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
                OnIsSyncNotRunningChanged(_isSyncNotRunning);
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

        private string _progressMessage;
        public string ProgressMessage
        {
            get { return _progressMessage; }
            set
            {
                _progressMessage = value;
                RaisePropertyChanged("ProgressMessage");
            }
        }

        [Dependency]
        public SynchronizeViewModel(TabSyncService syncService, MessageHub hub)
        {
             this.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator = this.ProgressIndicator;
            SyncService = syncService;
            SyncService.Progress += (sender, i) =>
            {
                Progress = i;
                DownloadedTabs = SyncService.Downloaded;
                UploadedTabs = SyncService.Uploaded;
            };

            SyncService.ProgressMessage += (sender, i) =>
            {
                SetProgressIndicator(!IsSyncNotRunning, i.GetMessage() ); 
            };

            SyncService.Complete += (o, e) =>
            {
                
                hub.RaiseTabsDownloaded();
                hub.RaiseTabsRefreshed();
                IsSyncNotRunning = true;
                SetProgressIndicator(true, DownloadedTabs.ToString() + " tabs downloaded - " + UploadedTabs.ToString() + " tabs uploaded.");
                SystemTray.ProgressIndicator.IsIndeterminate = false;
            };
            IsSyncNotRunning = true;
        }

        public ICommand LaunchSync
        {
            get
            {
                return new ExecuteCommand(() =>
                {
                    this.ProgressIndicator = new ProgressIndicator();
                    IsSyncNotRunning = false;
                    SetProgressIndicator(!IsSyncNotRunning, "Connecting to SkyDrive..."); 
                    SyncService.Synchronize(DownloadAll);
                });
            }
        }


        #region Helper Methods

        /// <summary>
        /// Helper Method to set the progress indicator.
        /// </summary>
        /// <param name="isVisible"></param>
        /// <param name="indicatorText"></param>
        private void SetProgressIndicator(bool isVisible, [Optional]string indicatorText)
        {
            
          if (!(String.IsNullOrEmpty(indicatorText)))
            SystemTray.ProgressIndicator.Text = indicatorText;

            SystemTray.ProgressIndicator.IsIndeterminate = isVisible;
            SystemTray.ProgressIndicator.IsVisible = isVisible;
        }

        private void OnIsSyncNotRunningChanged(bool isSynchNotRunning)
        {
            SetProgressIndicator(!isSynchNotRunning);
        }
        #endregion
    }
}
