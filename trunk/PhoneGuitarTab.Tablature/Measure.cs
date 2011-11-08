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
    public class Measure
    {
        //TODO: Metadata of measure

        public int StringsNumber { get; set; } //TODO: ambiguous usage

        public Byte NumeratorSignature { get; set; }
        public Byte DenominatorSignature { get; set; }

        public List<Beat> Beats { get; set; }

        public Measure()
        {
            Beats = new List<Beat>();
        }
    }
}
