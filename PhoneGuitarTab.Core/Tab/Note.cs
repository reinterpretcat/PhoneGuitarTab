using System;
using System.ComponentModel;
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
    /// <summary>
    /// represents a note
    /// </summary>
    public class Note 
    {
        //TODO header

        public bool IsLegato { get; set; }
        //public bool IsDotted { get; set; }

        public Bend Bend { get; set; }
        public Slide Slide { get; set; }
        private string _fret;
        public string Fret
        {
            get { return _fret; }
            set
            {
                _fret = value;
                
            }
        }
    }
}
