using System.Collections.Generic;
using System.Linq;

namespace PhoneGuitarTab.Tablatures.Models
{
    public class Voice
    {
        public static int DirectionNone = 0;
        public static int DirectionUp = 1;
        public static int DirectionDown = 2;

        public Beat Beat { get; set; }
        public Duration Duration { get; set; }
        public List<Note> Notes { get; set; }
        public int Index { get; set; }
        public int Direction { get; set; }
        public bool IsEmpty { get; set; }

        public bool IsRestVoice
        {
            get { return !Notes.Any(); }
        }

        public Voice(int index)
        {
            Duration = new Duration();
            Notes = new List<Note>();
            Index = index;
            IsEmpty = true;
            Direction = DirectionNone;
        }
    }
}