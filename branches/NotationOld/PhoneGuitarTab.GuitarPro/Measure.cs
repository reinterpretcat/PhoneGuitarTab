using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhoneGuitarTab.GuitarPro
{
    public class Measure
    {
        internal byte Header { get; set; }

        public Byte NumeratorSignature { get; set; }
        public Byte DenominatorSignature { get; set; }
        public bool BeginRepeat { get; set; }
        public Byte EndRepeat { get; set; }
        public Byte NumberAltEnding { get; set; }
        public Marker PresenceMarker { get; set; }
        public Key TonalityMeasure { get; set; }
        public bool PresenceDoubleBar { get; set; }

        internal static Measure GetMeasureFromPrevious(Measure previous)
        {
            return previous == null
                       ? new Measure()
                       : new Measure()
                             {
                                 DenominatorSignature = previous.DenominatorSignature,
                                 NumeratorSignature = previous.NumeratorSignature,
                                 TonalityMeasure = previous.TonalityMeasure
                             };
        }
        
       
    }
}
