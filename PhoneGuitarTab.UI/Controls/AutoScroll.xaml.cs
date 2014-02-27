using System;
using System.Collections.Generic;
using System.Dynamic;
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
        public bool isScrolling { get; set; }

        private bool isFirstLaunch = true;

        public void invokeAutoScroll(object sender)
        {
            //stop the previous invocation.
            this.stopAutoScroll(this);

            //if value is not set to 0 then slide.
            if (slideControl.Value != 0)
            {
                this.Browser.InvokeScript("slide", ((10 - System.Convert.ToInt16(slideControl.Value)) * 14).ToString());
                this.isScrolling = true;
            }
            
          
            if (this.isFirstLaunch)
            {
                //animate for the first launch
                this.fadein.Begin();

            }
            else 
            {
                //if invoked from outside, show info message.
                if (!sender.GetType().Equals(typeof(AutoScroll)))
                {
                    this.info.Text = "resume";
                    this.animateinfo.Begin();
                }
            }

            this.isFirstLaunch = false;
        }
        public void stopAutoScroll(object sender)
        {
            this.Browser.InvokeScript("stopSlide");
            this.isScrolling = false;

            //if invoked from outside, show info message.
            if (!sender.GetType().Equals(typeof(AutoScroll)))
            {
                this.info.Text = "paused";
                this.animateinfo.Begin();
            }

        }


        #region Slide Control Events
        private void slideControl_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            this.invokeAutoScroll(this);
        }

        private void slideControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.slideControl.Opacity = 1;
            this.slow.Opacity = 1;
            this.fast.Opacity = 1;
        }

        private void slideControl_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
           
            this.fadeout.Begin();
        }
        #endregion

       
    }
}
