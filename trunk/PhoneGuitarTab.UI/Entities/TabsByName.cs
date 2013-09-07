using PhoneGuitarTab.Data;
using System.Collections.ObjectModel;
using System.Linq;

namespace PhoneGuitarTab.UI.Entities
{
    using PhoneGuitarTab.Core.Dependencies;

    public class TabsByName : TabsGroupsCollection
    {
        #region Constructors

        public TabsByName(ObservableCollection<TabEntity> allTabs, IDataContextService database)
            : base(database)
        {
            Tabs = allTabs;
            Initialize();
        }

        public TabsByName(IDataContextService database): base(database)
        {
            Tabs = new ObservableCollection<TabEntity>((from Tab tab in Database.Tabs
                       orderby tab.Name
                       select tab).Select(tab => tab.CreateEntity()));

            Initialize();
        }

        #endregion Constructors
    }
}
