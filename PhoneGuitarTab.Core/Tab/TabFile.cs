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

namespace PhoneGuitarTab.Core.Tab
{
    public class TabFile
    {
        public Header Header { get; set; }
        public IList<Track> Tracks { get; set; }
        public TabFile()
        {
            Tracks = new List<Track>();
        }
    }
}
