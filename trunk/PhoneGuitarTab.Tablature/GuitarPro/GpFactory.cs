using System;
using System.Collections.Generic;
using System.IO;
using PhoneGuitarTab.Tablature.GuitarPro.VersionSpecific;

namespace PhoneGuitarTab.Tablature.GuitarPro
{
    public class GpFactory
    {
        public static GpFile CreateFile(Stream fileStream)
        {
            GpInputStream gpStream = new GpInputStream(fileStream);
            Version version = gpStream.GetVersionFromString();
            switch (version.Major)
            {
                case "5":
                    return Gp5InputStream.Create(fileStream, version);
                case "4":
                    return Gp4InputStream.Create(fileStream, version);
                case "3":
                    return Gp3InputStream.Create(fileStream, version);
            }
            return null;
        }
    }
}
