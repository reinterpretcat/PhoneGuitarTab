using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PhoneGuitarTab.UI.Notation.Persistence
{
    public class TabByName : List<TabInGroup>
    {
        private static readonly string Groups = "#abcdefghijklmnopqrstuvwxyz";

        public List<Tab> AllTabs { get; private set; }

        public TabByName(List<Tab> alltabs)
        {
            AllTabs = alltabs;

            alltabs.Sort(Tab.CompareByName);

            Dictionary<string, TabInGroup> groups = new Dictionary<string, TabInGroup>();

            foreach (char c in Groups)
            {
                TabInGroup tab = new TabInGroup(c.ToString());
                this.Add(tab);
                groups[c.ToString()] = tab;
            }

            foreach (Tab t in alltabs)
            {
                groups[Tab.GetNameKey(t)].Add(t);
            }

        }
    }
}
