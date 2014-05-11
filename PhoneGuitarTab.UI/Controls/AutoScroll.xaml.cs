using System.Windows.Controls;
using Microsoft.Phone.Controls;

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
        ///     Flag to resume/stop scrolling when tapped.
        /// </summary>
        public bool isScrolling { get; set; }

        private bool isFirstLaunch = true;

        public void invokeAutoScroll(object sender)
        {
            //stop the previous invocation.
            stopAutoScroll(this);

            //if value is not set to 0 then slide.
            if (slideControl.Value != 0)
            {
                Browser.InvokeScript("slide", ((10 - System.Convert.ToInt16(slideControl.Value))*35).ToString());
                isScrolling = true;
            }


            if (isFirstLaunch)
            {
                //animate for the first launch
                fadein.Begin();
            }
            else
            {
                //if invoked from outside, show info message.
                if (!sender.GetType().Equals(typeof (AutoScroll)))
                {
                    info.Text = "resume";
                    animateinfo.Begin();
                }
            }

            isFirstLaunch = false;
        }

        public void stopAutoScroll(object sender)
        {
            Browser.InvokeScript("stopSlide");
            isScrolling = false;

            //if invoked from outside, show info message.
            if (!sender.GetType().Equals(typeof (AutoScroll)))
            {
                info.Text = "paused";
                animateinfo.Begin();
            }
        }

        #region Slide Control Events

        private void slideControl_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            invokeAutoScroll(this);
        }

        private void slideControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            slideControl.Opacity = 1;
            slow.Opacity = 1;
            fast.Opacity = 1;
        }

        private void slideControl_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            fadeout.Begin();
        }

        #endregion
    }
}