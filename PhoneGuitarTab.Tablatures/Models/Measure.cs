using System.Collections.Generic;

namespace PhoneGuitarTab.Tablatures.Models
{
    public class Measure
    {
        public static int ClefTreble = 1;
        public static int ClefBass = 2;
        public static int ClefTenor = 3;
        public static int ClefAlto = 4;
        public static int DefaultClef = ClefTreble;
        public static int DefaultKeySignature = 0;

        public MeasureHeader Header { get; set; }
        public Track Track { get; set; }

        public int Clef { get; set; }
        public int KeySignature { get; set; }

        public List<Beat> Beats { get; set; }

        public Measure(MeasureHeader header)
        {
            Header = header;
            Clef = DefaultClef;
            KeySignature = DefaultKeySignature;
            Beats = new List<Beat>();
        }
    }
}