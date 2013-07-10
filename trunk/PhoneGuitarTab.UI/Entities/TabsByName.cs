using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PhoneGuitarTab.UI.Entities
{
    public class TabsByName : TabsCollection
    {
        #region Constructors

        public TabsByName(ObservableCollection<TabEntity> allTabs)
        {
            Tabs = allTabs;
            Initialize();
        }

        public TabsByName()
        {
            IDataContextService database = Container.Resolve<IDataContextService>();
            
            Tabs = new ObservableCollection<TabEntity>((from Tab tab in database.Tabs
                       orderby tab.Name
                       select tab).Select(tab => tab.CreateEntity()));

            Initialize();
        }

        #endregion Constructors
    }
}
