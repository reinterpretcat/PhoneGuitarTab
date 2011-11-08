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
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Phone.Info;

namespace PhoneGuitarTab.UI.Notation
{
  /// <summary>
  /// Helper class for showing current memory usage
  /// </summary>
  public static class MemoryDiagnosticsHelper
  {
    static Popup popup;
    static TextBlock currentMemoryBlock;
    static DispatcherTimer timer;
    static bool forceGc;

    public static void Start(bool forceGc)
    {
      MemoryDiagnosticsHelper.forceGc = forceGc;

      CreatePopup();
      CreateTimer();
      ShowPopup();
      StartTimer();
    }

    private static void ShowPopup()
    {
      popup.IsOpen = true;
    }

    private static void StartTimer()
    {
      timer.Start();
    }

    private static void CreateTimer()
    {
      if (timer != null)
        return;

      timer = new DispatcherTimer();
      timer.Interval = TimeSpan.FromMilliseconds(300);
      timer.Tick += new EventHandler(timer_Tick);
    }

    static void timer_Tick(object sender, EventArgs e)
    {
      if (forceGc)
        GC.Collect();

      long mem = (long)DeviceExtendedProperties.GetValue("ApplicationCurrentMemoryUsage");
      currentMemoryBlock.Text = string.Format("{0:N}", mem / 1024);
    }

    private static void CreatePopup()
    {
      if (popup != null)
        return;

      popup = new Popup();
      double fontSize = (double)App.Current.Resources["PhoneFontSizeSmall"] - 2;
      Brush foreground = (Brush)App.Current.Resources["PhoneForegroundBrush"];
      StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal, Background = (Brush)App.Current.Resources["PhoneSemitransparentBrush"] };
      currentMemoryBlock = new TextBlock { Text = "---", FontSize = fontSize, Foreground = foreground };
      sp.Children.Add(new TextBlock { Text = "Mem(k): ", FontSize = fontSize, Foreground = foreground });
      sp.Children.Add(currentMemoryBlock);
      sp.RenderTransform = new CompositeTransform { Rotation = 90, TranslateX = 480, TranslateY = 420, CenterX = 0, CenterY = 0 };
      popup.Child = sp;
    }

    public static void Stop()
    {
      HidePopup();
      StopTimer();
    }

    private static void StopTimer()
    {
      timer.Stop();
    }

    private static void HidePopup()
    {
      popup.IsOpen = false;
    }
  }
}
