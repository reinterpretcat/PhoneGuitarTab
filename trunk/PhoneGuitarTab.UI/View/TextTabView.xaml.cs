using System;
using PhoneGuitarTab.UI.ViewModel;
using Microsoft.Phone.Controls;


namespace PhoneGuitarTab.UI
{
    using PhoneGuitarTab.Core.Views;

    public partial class TextTabView : ViewPage
    {
        public TextTabView()
        {
            InitializeComponent();
          
            tabWebBrowser.Loaded += delegate
                {
                    var viewModel = DataContext as TextTabViewModel;
                    if (viewModel.TabContent != null)
                        tabWebBrowser.NavigateToString(viewModel.TabContent);
                  
                };
            
        }

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
            this.slideControl.Opacity = 0.1;
        }

       private void invokeAutoScroll()
        {
            tabWebBrowser.InvokeScript("stopSlide");
            if (slideControl.Value != 0)
                tabWebBrowser.InvokeScript("slide", ((10 - System.Convert.ToInt16(slideControl.Value)) * 8).ToString());       
        }


       
    }
}