using System;
using System.Collections.Generic;
using PhoneGuitarTab.Data;

namespace PhoneGuitarTab.UI.Controls
{
    public class TabInGroup : List<TabEntity>
    {
        public TabInGroup(string category)
        {
            Key = category;
        }

        public string Key { get; set; }

        public bool HasItems { get { return Count > 0; } }
    }
}
