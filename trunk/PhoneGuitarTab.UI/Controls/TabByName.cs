using System;
using System.Collections.Generic;
using System.Linq;
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;


namespace PhoneGuitarTab.UI.Controls
{
    public class TabByName : List<TabInGroup>
    {
        private static readonly string Groups = "#abcdefghijklmnopqrstuvwxyz";

        public List<TabEntity> AllTabs { get; private set; }

        public TabByName(List<TabEntity> allTabs)
        {
            AllTabs = allTabs;
            Initialize();
        }

        public TabByName()
        {
            IDataContextService database = Container.Resolve<IDataContextService>();
            
            AllTabs = (from Tab t in database.Tabs
                       orderby t.Name
                       select t).Select(t=> t.CreateEntity()).ToList();
            Initialize();
        }

        private void Initialize()
        {
            Dictionary<string, TabInGroup> groups = new Dictionary<string, TabInGroup>();
            foreach (char c in Groups)
            {
                TabInGroup tab = new TabInGroup(c.ToString());
                this.Add(tab);
                groups[c.ToString()] = tab;
            }

            foreach (TabEntity t in AllTabs)
            {
                groups[t.GetNameKey()].Add(t);
            }
        }
    }
}
