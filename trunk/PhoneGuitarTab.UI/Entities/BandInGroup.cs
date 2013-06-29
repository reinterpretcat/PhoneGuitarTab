using System;
using System.Collections.Generic;
using PhoneGuitarTab.Data;

namespace PhoneGuitarTab.UI.Entities
{
    public class BandInGroup : List<PhoneGuitarTab.Core.Tuple<int, Group>>
    {
        public BandInGroup(string category)
        {
            Key = category;
        }

        public string Key { get; set; }

        public bool HasItems { get { return Count > 0; } }
    }
}
