using System.Collections.Generic;
using System.Linq;
using PhoneGuitarTab.Core.Primitives;
using PhoneGuitarTab.UI.Data;

namespace PhoneGuitarTab.UI.Entities
{
    public class BandByName : List<BandInGroup>
    {
        private const string Groups = "#abcdefghijklmnopqrstuvwxyz";
        private readonly Dictionary<string, BandInGroup> _bandGroupsDictionary;

        private IDataContextService Database { get; set; }

        public BandByName(IDataContextService database)
        {
            Database = database;

            var allGroups = (from Group g in Database.Groups
                orderby g.Name
                select g).ToList();

            _bandGroupsDictionary = new Dictionary<string, BandInGroup>();
            CreateEmptyGroups(_bandGroupsDictionary);


            foreach (Group bandGroup in allGroups)
            {
                //get count of tab for this group
                var count = (from Tab t in Database.Tabs
                    where t.Group.Id == bandGroup.Id
                    select t).Count();

                _bandGroupsDictionary[bandGroup.GetNameKey()].Add(new ObservableTuple<int, Group>
                    (count, bandGroup));
            }
        }

        internal void DecreaseTabCount(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return;

            var firstChar = groupName.ToLower()[0];
            var groupKey = ((firstChar < 'a' || firstChar > 'z') ? '#' : firstChar);
            var band = _bandGroupsDictionary[groupKey.ToString()].Where(g => g.Item2.Name == groupName).FirstOrDefault();

            if (band != null)
            {
                var tabCount = band.Item1 - 1;
                band.Item1 = tabCount;
                if (tabCount == 0)
                    _bandGroupsDictionary[groupKey.ToString()].Remove(band);
            }
        }

        #region Helper methods

        private void CreateEmptyGroups(Dictionary<string, BandInGroup> bandGroupsDictionary)
        {
            foreach (char c in Groups)
            {
                BandInGroup group = new BandInGroup(c.ToString());
                Add(group);
                bandGroupsDictionary[c.ToString()] = group;
            }
        }

        #endregion Helper methods
    }
}