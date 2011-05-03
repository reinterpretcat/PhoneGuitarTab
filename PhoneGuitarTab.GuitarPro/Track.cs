using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace PhoneGuitarTab.GuitarPro
{
    public class Track
    {
        //The first byte is the track's header.  It precises the track's attributes:
        internal byte Header { get; set; }

        public bool IsDrumsTrack { get; set; }
        public bool Is12StringedGuitarTrack { get; set; }
        public bool IsBanjoTrack { get; set; }

        //A 40 characters long string containing the track's name.
        public string Name { get; set; }

        //An integer equal to the number of strings of the track.
        public int StringNumber { get; set; }

        //The tuning of the strings is stored as a 7-integers table, the "Number of
	    //strings" first integers being really used. The strings are stored from
	    //the highest to the lowest.
        public int[] StringTuning { get; set; }

        //The number of the MIDI port used.
        public int Port { get; set; }

        //The number of the MIDI channel used. The channel 10 is the drums channel.
        public int Channel { get; set; }

        //The number of the MIDI channel used for effects.
        public int ChannelEffects { get; set; }

        //The number of frets of the instrument.
        public int FretsNumber { get; set; }

        //The number of frets of the instrument.
        public int CapoHeight { get; set; }


        public Color Color { get; set; }

        public Track()
        {
            StringTuning = new int[7];
        }
       
    }
}
