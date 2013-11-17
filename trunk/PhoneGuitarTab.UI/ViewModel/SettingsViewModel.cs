
using System.Windows.Input;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Views.Commands;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class SettingsViewModel : PhoneGuitarTab.Core.Views.ViewModel
    {
        [Dependency]
        public TabSyncService SyncService { get; set; }

        public ICommand LaunchSync
        {
            get
            {
                return new ExecuteCommand(() =>
                  SyncService.Synchronize());
            }
        }
    }
}
