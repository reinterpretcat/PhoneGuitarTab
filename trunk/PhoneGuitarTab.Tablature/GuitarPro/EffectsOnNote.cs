using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhoneGuitarTab.Tablature.GuitarPro
{
    public class EffectsOnNote
    {
        internal byte[] Header { get; set; }

        public Bend Bend { get; set; }
        public GraceNote GraceNote { get; set; }
        public bool HammerOnPullOff { get; set; }
        public Harmonic Harmonic { get; set; }
        public bool LeftHandVibrato { get; set; }
        public bool LetRing { get; set; }
        public bool PalmMute { get; set; }
        public Slide Slide { get; set; }
        public bool Staccato { get; set; }
        public Duration TremoloPicking { get; set; }
        public Trill Trill { get; set; }
        public bool WideVibrato { get; set; }
    }
}
