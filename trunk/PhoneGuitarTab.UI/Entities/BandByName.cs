using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using System.Collections.Generic;
using System.Linq;

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
            {
                var tabCount = band.Item1 - 1;
                band.Item1 = tabCount;
                if (tabCount == 0)
                    bandGroupsDictionary[groupKey.ToString()].Remove(band);
            }
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
