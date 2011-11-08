using System;
using System.Collections.Generic;
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

namespace PhoneGuitarTab.Tablature
{
    /// <summary>
    /// Wraps beat
    /// </summary>
    public class Beat : INotifyPropertyChanged
    {
        //TODO: Metadata
        public Duration Duration { get; set; }
        public BeamType Beam { get; set; }
        public int Tuplet { get; set; }
        public bool IsDotted { get; set; }

        private List<Note> _notes;
        public List<Note> Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                NotifyPropertyChanged("Notes");
            }
        }

        public int _width;
        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                NotifyPropertyChanged("Width");
            }
        }

        //public List<Note> Notes { get; set; }
        public Beat()
        {
            Notes = new List<Note>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
