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
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;

namespace PhoneGuitarTab.UI.Entities
{
    public class BandByName : List<BandInGroup>
    {
        private static readonly string Groups = "#abcdefghijklmnopqrstuvwxyz";
       
        public BandByName()
        {
            IDataContextService database = Container.Resolve<IDataContextService>();

            var allGroups = (from Group g in database.Groups
                             orderby g.Name
                             select g).ToList();

            Dictionary<string, BandInGroup> groups = new Dictionary<string, BandInGroup>();

            foreach (char c in Groups)
            {
                BandInGroup group = new BandInGroup(c.ToString());
                this.Add(group);
                groups[c.ToString()] = group;
            }

           
            foreach (Group tabGroup in allGroups)
            {
                //get count of tab for this group
                var count = (from Tab t in database.Tabs
                             where t.Group.Id == tabGroup.Id
                            select t).Count();
                groups[tabGroup.GetNameKey()].Add(new PhoneGuitarTab.Core.Tuple<int, Group>
                                               (count, tabGroup));
            }

        }
    }
}
