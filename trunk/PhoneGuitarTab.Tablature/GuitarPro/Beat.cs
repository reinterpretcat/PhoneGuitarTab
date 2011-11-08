using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhoneGuitarTab.Tablature.GuitarPro
{
    public class Beat
    {
        internal byte Header { get; set; }
        public bool EmptyBit { get; set; }
        public bool RestBit { get; set; }
        public bool DottedNotes { get; set; }
        public Duration Duration { get; set; }
        public int NTuplet { get; set; }
        public ChordDiagram ChordDiagram { get; set; }
        public string Text { get; set; }
        public EffectsOnBeat Effects { get; set; }
        public Note[] Notes { get; set; }
        public MixTableChange MixTableChange { get; set; }
        public bool[] Strings { get; set; }

        public Beat()
        {
            Strings = new bool[7];
        }
    }
}
