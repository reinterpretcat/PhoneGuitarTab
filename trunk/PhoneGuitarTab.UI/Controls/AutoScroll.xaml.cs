using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace PhoneGuitarTab.UI.Controls
{
    public partial class AutoScroll : UserControl
    {
        public AutoScroll()
        {
            InitializeComponent();
        }

        public WebBrowser Browser { get; set; }
        
        /// <summary>
        /// Flag to resume/stop scrolling when tapped.
        /// </summary>
        public bool isScrolling = false;


        public void invokeAutoScroll()
        {
            this.stopAutoScroll();
            if (slideControl.Value != 0)
            {
                this.Browser.InvokeScript("slide", ((10 - System.Convert.ToInt16(slideControl.Value)) * 8).ToString());
                this.isScrolling = true;
            }

        }
        public void stopAutoScroll()
        {
            this.Browser.InvokeScript("stopSlide");
            this.isScrolling = false;
        }


        #region Slide Control Events
        private void slideControl_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            this.invokeAutoScroll();
        }

        private void slideControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.slideControl.Opacity = 1;
        }

        private void slideControl_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.slideControl.Opacity = 0.07;
        }
        #endregion

    }
}
