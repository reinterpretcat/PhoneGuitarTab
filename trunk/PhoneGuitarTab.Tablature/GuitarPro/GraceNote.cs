using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhoneGuitarTab.Tablature.GuitarPro
{
    public class GraceNote
    {
        public Duration Duration { get; set; }
        public Dynamic Dynamic { get; set; }
        public int Fret { get; set; }
        public GraceNoteTransition Transition { get; set; }
    }
}
