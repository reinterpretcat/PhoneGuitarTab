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

            this.slider.Browser = tabWebBrowser; 
        }

       
        /// <summary>
        /// Event to Stop / Resume the scrolling when tapped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabWebBrowser_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.slider.isScrolling)
                this.slider.stopAutoScroll();
            else
                this.slider.invokeAutoScroll();
        }


      
    }
}