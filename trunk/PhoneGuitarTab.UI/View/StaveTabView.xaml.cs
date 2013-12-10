namespace PhoneGuitarTab.UI.View
{
    using System;
    using System.Windows;
    using Microsoft.Phone.Controls;
    using PhoneGuitarTab.UI.ViewModel;

    // TODO move code-behind to viewmodel
    public partial class StaveTabView : PhoneApplicationPage
    {
        // Url of Home page
        private string SandboxUri = "/Html/sandbox.html";

        public StaveTabView()
        {
            this.InitializeComponent();
            this.slider.Browser = this.Browser; 
        }

        private void Browser_Loaded(object sender, RoutedEventArgs e)
        {
            Browser.IsScriptEnabled = true;
            Browser.Navigate(new Uri(SandboxUri, UriKind.Relative));
        }

        private void BackApplicationBar_Click(object sender, EventArgs e)
        {
            Browser.InvokeScript("prevTrack");
        }

        // Navigates forward in the web browser's navigation stack, not the applications.
        private void ForwardApplicationBar_Click(object sender, EventArgs e)
        {
            Browser.InvokeScript("nextTrack");
        }

        // Navigates forward in the web browser's navigation stack, not the applications.
        private void ScaleApplicationBar_Click(object sender, EventArgs e)
        {
            Browser.InvokeScript("scaleChange");
        }

        // Navigates to the initial "home" page.
        private void HomeMenuItem_Click(object sender, EventArgs e)
        {
            (DataContext as StaveTabViewModel).NavigateToHome();
        }
        
        // Handle navigation failures.
        private void Browser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            MessageBox.Show("Navigation to this page failed, check your internet connection");
        }

        private void Browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (e.Value.StartsWith("onReady"))
            {
                string[] parameters = new [] { (DataContext as StaveTabViewModel).TabContent };

                // TODO json tab is specific for ug. Convert it to gp through download step 
                Browser.InvokeScript("readBase64", parameters);
                OrientationChanged += (_, __) => Browser.InvokeScript("showTab");

            }
        }

     
      
        //Autoscroll - Tap to slide/stop.
        private void Browser_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.slider.isScrolling)
                this.slider.stopAutoScroll();
            else
                this.slider.invokeAutoScroll();
        }

    }
}