using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhoneGuitarTab.Tablature.GuitarPro
{
    public class Lyrics
    {
        internal const byte LyricsCount = 5;

        public int TrackNumber { get; set; }
        public int[] MeasureNumber { get; set; }
        public string[] Lines { get; set; }
        public Lyrics()
        {
            MeasureNumber = new int[LyricsCount];
            Lines = new string[LyricsCount];
        }
    }
}
