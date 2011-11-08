using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PhoneGuitarTab.Tablature
{
    public class Track
    {
        //TODO: Metadata of track

        public int Index { get; set; }
        public string Name { get; set; }

        public bool IsDrum { get; set; }
        public int StringNumber { get; set; }

        public IList<Measure> Measures { get; set; }
        public Track()
        {
            Measures = new List<Measure>();
            //StringNumber = 6;
        }
    }
}
