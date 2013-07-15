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
        private Dictionary<string, BandInGroup> bandGroupsDictionary;

        public BandByName()
        {
            IDataContextService database = Container.Resolve<IDataContextService>();
            var allGroups = (from Group g in database.Groups
                             orderby g.Name
                             select g).ToList();

            bandGroupsDictionary = new Dictionary<string, BandInGroup>();
            CreateEmptyGroups(bandGroupsDictionary);

           
            foreach (Group bandGroup in allGroups)
            {
                //get count of tab for this group
                var count = (from Tab t in database.Tabs
                             where t.Group.Id == bandGroup.Id
                            select t).Count();

                bandGroupsDictionary[bandGroup.GetNameKey()].Add(new Tuple<int, Group>
                                               (count, bandGroup));
            }
        }


        internal void DecreaseTabCount(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return;

            var firstChar = groupName.ToLower()[0];
            var groupKey = ((firstChar < 'a' || firstChar > 'z') ? '#' : firstChar);
            var band = bandGroupsDictionary[groupKey.ToString()].Where(g => g.Item2.Name == groupName).FirstOrDefault();

            if (band != null)
                band.Item1 = band.Item1 - 1;
        }


        #region Helper methods

        private void CreateEmptyGroups(Dictionary<string, BandInGroup> bandGroupsDictionary)
        {
            foreach (char c in Groups)
            {
                BandInGroup group = new BandInGroup(c.ToString());
                this.Add(group);
                bandGroupsDictionary[c.ToString()] = group;
            }
        }

        #endregion Helper methods
    }
}
