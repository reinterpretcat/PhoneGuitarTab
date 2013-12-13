﻿namespace PhoneGuitarTab.UI.View
{
    using System;
    using System.Windows;
    using System.Runtime.InteropServices;
    using Microsoft.Phone.Controls;
    using PhoneGuitarTab.UI.ViewModel;
    using Microsoft.Phone.Shell;

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

        /// <summary>
        /// Helper Method to set the progress indicator.
        /// </summary>
        /// <param name="isVisible"></param>
        /// <param name="indicatorText"></param>
        private void SetProgressIndicator(bool isVisible, [Optional]string indicatorText)
        {
            SystemTray.IsVisible = isVisible;
            SystemTray.ProgressIndicator.Text = indicatorText;
            SystemTray.ProgressIndicator.IsIndeterminate = isVisible;
            SystemTray.ProgressIndicator.IsVisible = isVisible;
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

        private void Browser_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {   
           
            //Autoscroll - Tap to slide/stop.
            if (this.slider.isScrolling)
                this.slider.stopAutoScroll();
            else
                this.slider.invokeAutoScroll();
        }


        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Set Progress Indicator on PageLoad.
            SystemTray.SetOpacity(this, 0.7);
            var progressIndicator = new ProgressIndicator();
            SystemTray.SetProgressIndicator(this, progressIndicator);
            SetProgressIndicator(true, "Loading guitar pro content...");
        }

        private void Browser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            SetProgressIndicator(false);
        }
        
       
       
    }
}