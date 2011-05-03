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

namespace PhoneGuitarTab.UI.Notation.Tests
{
    public class TestHelper
    {
        public static Stream GetGp5TupleSample()
        {
            return GetResource("PhoneGuitarTab.UI.Notation.Tests.Tuple.gp5");
        }

        public static Stream GetGp5Performance()
        {
            return GetResource("PhoneGuitarTab.UI.Notation.Tests.PerformanceTest.gp5");
        }

        public static Stream GetGp5SmallSample()
        {
            return GetResource("PhoneGuitarTab.UI.Notation.Tests.OneMeasure.gp5");
        }
        public static Stream GetGp5MediumSample()
        {
            return GetResource("PhoneGuitarTab.UI.Notation.Tests.Opeth_Coil.gp5");
        }

        public static Stream GetGp5MediumSample2()
        {
            return GetResource("PhoneGuitarTab.UI.Notation.Tests.Aria.gp5");
        }

        public static Stream GetGp4SmallSample()
        {
            return GetResource("PhoneGuitarTab.UI.Notation.Tests.Aragonise.gp4");
        }
        public static Stream GetGp4MediumSample()
        {
            return GetResource("PhoneGuitarTab.UI.Notation.Tests.Marty_Friedman.gp4");
        }

        public static Stream GetGp4MediumSample2()
        {
            return GetResource("PhoneGuitarTab.UI.Notation.Tests.GodsTower_Evil.gp4");
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
