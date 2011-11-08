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
using PhoneGuitarTab.GuitarPro;

namespace PhoneGuitarTab.Tests
{
    [TestFixture]
    public class Gp4Parser
    {
        [Test]
        public void ReadGp4()
        {
            var stream = TestHelper.GetGp4Sample();
            GpFile file = GpFactory.CreateFile(stream);

            TestHelper.AssertGp4Sample(file);
         
        }
    }
}
