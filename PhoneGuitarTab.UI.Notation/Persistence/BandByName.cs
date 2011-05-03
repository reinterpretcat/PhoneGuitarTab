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
    public class BandByName : List<BandInGroup>
    {
        private static readonly string Groups = "#abcdefghijklmnopqrstuvwxyz";
       
        public BandByName(List<Group> allGroups)
        {
            allGroups.Sort(Group.CompareByName);

            Dictionary<string, BandInGroup> groups = new Dictionary<string, BandInGroup>();

            foreach (char c in Groups)
            {
                BandInGroup group = new BandInGroup(c.ToString());
                this.Add(group);
                groups[c.ToString()] = group;
            }

            foreach (Group g in allGroups)
            {
                groups[Group.GetNameKey(g)].Add(new Tuple<int, Group>
                    (RepositoryHelper.GetTabsCount(g.Id), g));
            }

        }
    }
}
