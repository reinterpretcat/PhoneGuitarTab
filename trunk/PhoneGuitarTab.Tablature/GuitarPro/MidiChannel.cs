using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhoneGuitarTab.Tablature.GuitarPro
{
    public class MidiChannel
    {
        public int Instrument { get; set; }
        public byte Volume { get; set; }
        public byte Balance { get; set; }
        public byte Chorus { get; set; }
        public byte Reverb { get; set; }
        public byte Phaser { get; set; }
        public byte Tremolo { get; set; }
        public byte Blank1 { get; set; }
        public byte Blank2 { get; set; }

       

    }
}
