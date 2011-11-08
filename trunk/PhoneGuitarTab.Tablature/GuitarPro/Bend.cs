using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhoneGuitarTab.Tablature.GuitarPro
{
    public class Bend
    {
        public BendType Type { get; set; }
        public int Value { get; set; }

        public BendPoint[] Points { get; set; }

       

    }
}
