using System.Collections.Generic;
using System.IO;
using System.Linq;
using PhoneGuitarTab.Tablature;

namespace PhoneGuitarTab.Tablature.GuitarPro
{
    public class GpFile
    {
        public Header Header { get; set; }
        public Body Body { get; set; }

        internal GpFile() { }

       /* public static GpFile Create(Stream stream)
        {
            //TODO: detect version
            return Gp5InputStream.Create(stream);
        }*/

    }
}
