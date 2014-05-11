using System.Collections.ObjectModel;
using System.Linq;
using PhoneGuitarTab.UI.Data;

namespace PhoneGuitarTab.UI.Entities
{
    public class TabsByName : TabsGroupsCollection
    {
        #region Constructors

        public TabsByName(ObservableCollection<TabEntity> allTabs, IDataContextService database)
            : base(database)
        {
            Tabs = allTabs;
            Initialize();
        }

        public TabsByName(IDataContextService database, bool createEmpty = false) : base(database)
        {
            if (createEmpty)
            {
                Tabs = new ObservableCollection<TabEntity>();
            }
            else
            {
                Tabs = new ObservableCollection<TabEntity>((from Tab tab in Database.Tabs
                    orderby tab.Name
                    select tab).Select(tab => tab.CreateEntity()));
            }
            Initialize();
        }

        #endregion Constructors
    }
}