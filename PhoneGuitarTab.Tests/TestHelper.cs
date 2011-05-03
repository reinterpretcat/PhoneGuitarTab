using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using NUnit.Framework;
using PhoneGuitarTab.GuitarPro;

namespace PhoneGuitarTab.Tests
{
    internal static class TestHelper
    {

        public static void AssertGp4Sample(GpFile file)
        {
            //Check header
            Assert.AreEqual("Aragonise (Solo)", file.Header.Tablature.Title);
            Assert.AreEqual("Wolf Hoffmann", file.Header.Tablature.Interpret);
            Assert.AreEqual("Classical", file.Header.Tablature.Album);

            //check body
            Assert.AreEqual(18, file.Body.Measures.Length);
        }

        public static Stream GetGp4Sample()
        {
            return GetResource("PhoneGuitarTab.Tests.Samples.Aragonise.gp4");
        }

        public static Stream GetResource(string path)
        {
            try
            {
                return Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(String.Format("Cannot get {0} from resources", path), ex);
            }
        }
    }
}
