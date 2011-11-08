using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhoneGuitarTab.GuitarPro
{
    public class EffectsOnBeat
    {
        internal byte[] Header { get; set; }

        public bool Tapping { get; set; }
        public bool Slapping { get; set; }
        public bool Popping { get; set; }

        public Bend TremoloBar { get; set; }
        public Duration UpStroke { get; set; }
        public Duration DownStroke { get; set; }
        public bool HasRasgueado { get; set; }
        public PickStroke PickStroke { get; set; }

      
    }
}
