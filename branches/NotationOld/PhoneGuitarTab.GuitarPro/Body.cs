using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhoneGuitarTab.GuitarPro
{
    public class Body
    {
        public Measure[] Measures { get; set; }
        public Track[] Tracks { get; set; }
        public MeasureTrackPair[] MeasureTrackPairs { get; set; }
    }
}
