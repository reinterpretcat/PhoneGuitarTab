using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using NUnit.Framework;
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Core.Tab;
using PhoneGuitarTab.GuitarPro;

namespace PhoneGuitarTab.Tests
{
    [TestFixture]
    public class CoreWrappers
    {
        [Test(Description = "Check whether we can convert to core wrapper format")]
        public void CanConvertGp4ToFile()
        {
            var gpFile = TestHelper.GetGp4Sample();
            TabFile tabFile = TabFactory.CreateFromGp(gpFile);

            //asserts
            Assert.AreEqual(1, tabFile.Tracks.Count);
            Assert.AreEqual(18, tabFile.Tracks[0].Measures.Count);
        }
    }
}
