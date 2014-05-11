using System.Collections.Generic;
using System.Collections.ObjectModel;
using PhoneGuitarTab.UI.Data;

namespace PhoneGuitarTab.UI.Entities
{
    public abstract class TabsGroupsCollection : ObservableCollection<TabInGroup>
    {
        #region Fields

        private static readonly string Groups = "#abcdefghijklmnopqrstuvwxyz";
        private Dictionary<string, TabInGroup> groups;

        #endregion Fields

        protected TabsGroupsCollection(IDataContextService database)
        {
            Database = database;
        }

        #region Properties

        protected IDataContextService Database { get; private set; }

        public ObservableCollection<TabEntity> Tabs { get; set; }

        #endregion Properties

        #region Public methods

        public void RemoveTab(TabEntity tabToRemove)
        {
            if (tabToRemove != null)
            {
                groups[tabToRemove.GetNameKey()].Remove(tabToRemove);
            }
        }

        public TabEntity GetFirstTabInFirstNonEmptyGroup()
        {
            foreach (var group in this)
            {
                if (group.HasItems)
                    return group[0];
            }

            return null;
        }

        #endregion Public methods

        #region Helper methods

        protected void Initialize()
        {
            groups = new Dictionary<string, TabInGroup>();
            foreach (char c in Groups)
            {
                TabInGroup tab = new TabInGroup(c.ToString());
                Add(tab);
                groups[c.ToString()] = tab;
            }

            foreach (TabEntity t in Tabs)
            {
                groups[t.GetNameKey()].Add(t);
            }
        }

        #endregion Helper methods
    }
}