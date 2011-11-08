using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhoneGuitarTab.Tablature.GuitarPro
{
    public class MixTableElement
    {
        public bool ApplyToAllTracks { get; set; }
        public int ChangeDuration { get; set; }
        public int NewValue { get; set; }
    }
}
