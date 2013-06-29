using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using System.Collections.Generic;
using System.Linq;

namespace PhoneGuitarTab.UI.Entities
{
    public class TabsByName : TabsCollection
    {
        #region Constructors

        public TabsByName(List<TabEntity> allTabs)
        {
            Tabs = allTabs;
            Initialize();
        }

        public TabsByName()
        {
            IDataContextService database = Container.Resolve<IDataContextService>();
            
            Tabs = (from Tab tab in database.Tabs
                       orderby tab.Name
                       select tab).Select(tab => tab.CreateEntity()).ToList();

            Initialize();
        }

        #endregion Constructors
    }
}
