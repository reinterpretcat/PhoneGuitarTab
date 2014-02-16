using System;
using PhoneGuitarTab.UI.ViewModel;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;

namespace PhoneGuitarTab.UI
{
    using PhoneGuitarTab.Core.Views;
    using PhoneGuitarTab.UI.Entities;
    using System.Windows;

    public partial class TextTabView : ViewPage
    {
        private string TextTabUri = "/Html/texttab.html";
       
        public TextTabView()
        {
            InitializeComponent();
            this.slider.Browser = this.tabWebBrowser;
            
        }
     

       
        /// <summary>
        /// Event to Stop / Resume the scrolling when tapped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabWebBrowser_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.slider.Visibility == Visibility.Visible)
            {
                if (this.slider.isScrolling == true)
                    this.slider.stopAutoScroll();
                else
                    this.slider.invokeAutoScroll(); 
            }
        }

        private void tabWebBrowser_ScriptNotify(object sender, NotifyEventArgs e)
        {
             if (e.Value.StartsWith("onReady"))
             {
                var viewModel = DataContext as TextTabViewModel;

                 if (viewModel.TabContent != null)
                 {
                     tabWebBrowser.InvokeScript("pullTabContent", viewModel.TabContent);
                 }
             }
        }

        private void tabWebBrowser_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            tabWebBrowser.Navigate(new Uri(this.TextTabUri, UriKind.Relative));
        }

       

        private void AutoScrollApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            //Control Autoscroll behaviour
            if (this.slider.Visibility == Visibility.Visible)
            {
                this.slider.Visibility = Visibility.Collapsed;
                this.slider.stopAutoScroll();
            }
            else
            {
                this.slider.Visibility = Visibility.Visible;
                this.slider.invokeAutoScroll();
            }
        }

        private void PinToStartIconButton_Click(object sender, EventArgs e)
        {
            var viewModel = DataContext as TextTabViewModel;
            viewModel.PinTabToStart();
        }


       
       
    }
}