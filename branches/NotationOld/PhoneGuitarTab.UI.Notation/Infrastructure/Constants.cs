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

namespace PhoneGuitarTab.UI.Notation.Infrastructure
{
    internal static class Constants
    {
        public static int NoteHeight = 38;
        public static int NoteFontSize = 30;
        public static int NoteSizeOffset = NoteFontSize/3;
        public static int StemHeight = 40;

        public static Brush MeasureBrush = new SolidColorBrush(Colors.Black);
        public static Brush EffectsBrush = new SolidColorBrush(Colors.Black);
        public static Brush StringsBrush = new SolidColorBrush(Colors.Black);
    }
}
