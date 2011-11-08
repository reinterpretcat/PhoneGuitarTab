using System;
using System.IO;
using System.Text;

namespace PhoneGuitarTab.GuitarPro
{
    /// <summary>
    /// The title of the piece;
    /// The subtitle of the piece;
    /// The interpret of the piece;
    /// The album from which the piece was taken;
    /// The author of the piece;
    /// The copyright;
    /// The name of the author of the tablature;
    /// An 'instructional' line about the tablature.
    /// </summary>
    public class Tablature
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Interpret { get; set; }
        public string Album { get; set; }
        public string Author { get; set; }
        public string Copyright { get; set; }
        public string Tab { get; set; }
        public string Instructional { get; set; }

        public string[] Notice { get; set; }
        public bool IsTripletFeel { get; set; }

      

    }
}