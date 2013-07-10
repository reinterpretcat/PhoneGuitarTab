using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using System.Collections.ObjectModel;
using System.Linq;

namespace PhoneGuitarTab.UI.Entities
{
    public class TabsForBand : TabsCollection
    {
        #region Constructors

        public TabsForBand(int groupId)
        {
            IDataContextService database = Container.Resolve<IDataContextService>();
            
            Tabs = new ObservableCollection<TabEntity>((from Tab tab in database.Tabs
                       orderby tab.Name
                       select tab).Where(tab => tab.GroupId == groupId).Select(tab => tab.CreateEntity()));

            Initialize();
        }

        #endregion Constructors
    }
}
