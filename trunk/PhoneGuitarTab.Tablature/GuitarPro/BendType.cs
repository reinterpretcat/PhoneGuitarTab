using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhoneGuitarTab.Tablature.GuitarPro
{
    public enum BendType
    {
        None = 0,
        Bend,
        BendRelease,
        BendReleaseBend,
        Prebend,
        PrebendRelease,
        Dip,
        Dive,
        ReleaseUp,
        InvertedDip,
        Return,
        ReleaseDown
    }
}
