namespace PhoneGuitarTab.UI.Entities
{
    using System.Collections.ObjectModel;
    using System.Linq;

    using PhoneGuitarTab.Data;
    using PhoneGuitarTab.Core.Dependencies;

    public class TabsForBand : TabsGroupsCollection
    {
        #region Constructors

        public TabsForBand(int groupId, IDataContextService database)
            : base(database)
        {
            Tabs = new ObservableCollection<TabEntity>((from Tab tab in Database.Tabs
                       orderby tab.Name
                       select tab).Where(tab => tab.GroupId == groupId).Select(tab => tab.CreateEntity()));

            Initialize();
        }

        #endregion Constructors
    }
}
