
using System.Windows;
using System.Windows.Input;

using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Views.Commands;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class SettingsViewModel : PhoneGuitarTab.Core.Views.ViewModel
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

        [Dependency]
        public SettingsViewModel(TabSyncService syncService, MessageHub hub)
        {
            SyncService = syncService;
            SyncService.Progress += (sender, i) =>
            {
                Progress = i;
            };
            SyncService.Complete += (o, e) =>
            {
                IsSyncNotRunning = true;
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
                    SyncService.Synchronize();
                });
            }
        }
    }
}
