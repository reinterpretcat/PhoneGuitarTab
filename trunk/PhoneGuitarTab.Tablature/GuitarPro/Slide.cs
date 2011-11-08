using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhoneGuitarTab.Tablature.GuitarPro
{
    public enum Slide
    {
        FromAbove = -2,
        FromBelow = -1,
        NoSlide = 0,
        ShiftSlide = 1,
        LegatoSlide = 2,
        OutDownward = 3,
        OutUpward = 4
    }
}
