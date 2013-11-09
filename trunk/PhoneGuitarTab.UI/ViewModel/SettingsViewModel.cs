
using PhoneGuitarTab.Core.Cloud;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class SettingsViewModel: DataContextViewModel
    {
        [Dependency]
        public ICloudService CloudService { get; set; }

        public SettingsViewModel(IDataContextService database, MessageHub hub)
            : base(database, hub)
        {
        }
    }
}
