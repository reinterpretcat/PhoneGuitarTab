using System;
using System.IO;
using System.Text;

namespace PhoneGuitarTab.Tablature.GuitarPro
{
    public class Header
    {

        internal const UInt16 VersionSize = 31;
        
        public Version Version { get; set; }
        public Tablature Tablature { get; set; }
        public Lyrics Lyrics { get; set; }
        public AddInfo AdditionalInfo { get; set; }
    }
}