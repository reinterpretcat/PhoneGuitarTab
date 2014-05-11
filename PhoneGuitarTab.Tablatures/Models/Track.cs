using System;
using System.Collections.Generic;

namespace PhoneGuitarTab.Tablatures.Models
{
    public class Track
    {
        public static int MaxOffset = 24;
        public static int MinOffset = -24;

        public int Number { get; set; }
        public int Offset { get; set; }
        public bool Solo { get; set; }
        public bool Mute { get; set; }
        public String Name { get; set; }
        public List<Measure> Measures { get; set; }
        public List<GuitarString> Strings { get; set; }
        public Channel Channel { get; set; }
        public Color Color { get; set; }
        public Lyric Lyrics { get; set; }

        public Track()
        {
            Number = 0;
            Offset = 0;
            Solo = false;
            Mute = false;
            Name = "";
            Measures = new List<Measure>();
            Strings = new List<GuitarString>();
            Channel = new Channel();
            Color = new Color();
            Lyrics = new Lyric();
        }

        public bool IsPercussionTrack
        {
            get { return Channel.IsPercussionChannel; }
        }
    }
}