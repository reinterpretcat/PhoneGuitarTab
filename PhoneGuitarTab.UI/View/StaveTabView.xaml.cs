using System.Collections.ObjectModel;
using System.Windows.Controls;
using StaveTabViewModel = PhoneGuitarTab.UI.ViewModel.StaveTabViewModel;
using PhoneGuitarTab.UI.Entities;
namespace PhoneGuitarTab.UI.View
{
    using System;
    using System.Windows;
    using System.Runtime.InteropServices;
    using Microsoft.Phone.Controls;
    using PhoneGuitarTab.UI.ViewModel;
    using Microsoft.Phone.Shell;
    using PhoneGuitarTab.Core.Views;
    using System.Windows.Navigation;

    // TODO move code-behind to viewmodel
    public partial class StaveTabView : ViewPage
    {
        //Generic List for ListPicker data binding.
        public ObservableCollection<StaveTabViewModel.Track> Tracks;
        public bool _isNewPageInstance = false;
        //flag to prevent renavigation of browser while switching back and forth from listpicker's fullmode
        private bool isFirstLoad = true;
       
        // Url of Home page
        private const string SandboxUri = "/Html/sandbox.html";

        public StaveTabView()
        {
            this.InitializeComponent();
            this._isNewPageInstance = true;
            //Generic list that keeps track data - binded to the listpicker.
            this.Tracks = new ObservableCollection<StaveTabViewModel.Track>();
            this.ListPickerInstrument.DataContext = this.Tracks;
            
           //set slider's Browser property to current.
            this.slider.Browser = this.Browser;

            
            var viewModel = DataContext as StaveTabViewModel;
            
            //subscribe to AudioURLretrieved event.
            viewModel.AudioUrlRetrieved += this.AudioUrlRetrievedHandler;
         
        }

        private void Browser_Loaded(object sender, RoutedEventArgs e)
        {
            Browser.IsScriptEnabled = true;

            //Navigate only for the first load (to prevent renavigation while switching back and forth from listpicker's fullmode)
            //For all other Loads (after navigating back from instrument selection) become visible.
            if (isFirstLoad)
            {
                Browser.Navigate(new Uri(SandboxUri, UriKind.Relative));
               
            }

          
        }

        private void ScaleApplicationBar_Click(object sender, EventArgs e)
        {
            Browser.InvokeScript("scaleChange");
            if (!(this.ApplicationBar.Mode == ApplicationBarMode.Minimized))
                this.ApplicationBar.Mode = ApplicationBarMode.Minimized;
        }


        ////Appbar button to visible / hide for listpicker.
        private void InstrumentApplicationBar_Click(object sender, EventArgs e)
        {
          
            this.ListPickerInstrument.Open();

            if (!(this.ApplicationBar.Mode == ApplicationBarMode.Minimized))
                this.ApplicationBar.Mode = ApplicationBarMode.Minimized;
        }

        //Event that is fired after picking an instrument (latebinding after ListPicker populated)
        private void ListPickerInstrument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Browser.InvokeScript("changeInstrument", ListPickerInstrument.SelectedIndex.ToString());
           
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

            var viewModel = DataContext as StaveTabViewModel;        
            viewModel.Browser_ScriptNotify(sender, e);
             
        }


        private void Browser_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {   
           
            //Autoscroll - Tap to slide/stop.
            if (this.slider.Visibility == Visibility.Visible)
            {
                if (this.slider.isScrolling == true)
                    this.slider.stopAutoScroll(sender);
                else
                    this.slider.invokeAutoScroll(sender);
            }

            //Collapse ListPicker.
            this.ListPickerInstrument.Visibility = Visibility.Collapsed;

            if (!(this.ApplicationBar.Mode == ApplicationBarMode.Minimized))
                this.ApplicationBar.Mode = ApplicationBarMode.Minimized;
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

        //Helper method to populate the list with track data.
        private void FillTrackList(ObservableCollection<StaveTabViewModel.Track> trackList )
        {
            string trackLenght = (string)Browser.InvokeScript("getTrackCount");

            //for some strange reason track count returns as multiplied by 2 from JS side. Therefore here is a division by 2 for counter.
            for (int i = 0; i < Convert.ToInt16(trackLenght) / 2; i++)
            {
                trackList.Add(new StaveTabViewModel.Track((string)Browser.InvokeScript("getInstrumentName", i.ToString()), i, ""));
            }
          
        }


        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Set Progress Indicator on PageLoad.
            if (isFirstLoad)
            {
                SystemTray.SetOpacity(this, 0.7);
                var progressIndicator = new ProgressIndicator();
                SystemTray.SetProgressIndicator(this, progressIndicator);
                SetProgressIndicator(true, "Loading guitar pro content...");
            }
           
        }

        private void Browser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {   
            //stop progress indicator
            SetProgressIndicator(false);
           
            //Fill the tracklist just once in the first load.
            if (isFirstLoad)
            {
                this.FillTrackList(this.Tracks);  
                this.ListPickerInstrument.SelectionChanged += this.ListPickerInstrument_SelectionChanged;
                this.ListPickerInstrument.Visibility = Visibility.Visible;
                this.slider.MouseLeftButtonUp += Slider_Clicked;
                this.isFirstLoad = false;

                var viewModel = DataContext as StaveTabViewModel;
                viewModel.Browser_LoadCompleted(sender, e);
            }
           
        }

        
        private void Slider_Clicked(object sender, System.Windows.Input.MouseButtonEventArgs e )
        {
            //Hide listpicker when slider is clicked.
            this.ListPickerInstrument.Visibility=Visibility.Collapsed;
            
        }


        private void AutoScrollApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            //Control Autoscroll behaviour
            if (this.slider.Visibility == Visibility.Visible)
            {
                this.slider.Visibility = Visibility.Collapsed;
                this.slider.stopAutoScroll(sender);
            }
              else
            {
                this.slider.Visibility = Visibility.Visible;
                this.slider.invokeAutoScroll(sender);
            }

            if (!(this.ApplicationBar.Mode == ApplicationBarMode.Minimized))
                this.ApplicationBar.Mode = ApplicationBarMode.Minimized;
        }

        private void PinToStartIconButton_Click(object sender, EventArgs e)
        {
            var viewModel = DataContext as StaveTabViewModel;
            viewModel.PinTabToStart();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var viewModel = DataContext as StaveTabViewModel;
            viewModel.RatingService.IncreaseTabViewCount();
        }

        private void ViewPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            //Control the appbar visibility (in landscape mode it opens the buttons, reading tabs become harder)
            if ((this.Orientation == PageOrientation.LandscapeLeft) || (this.Orientation == PageOrientation.LandscapeRight))
            {
                this.ApplicationBar.IsVisible = false;
            }
            else { this.ApplicationBar.IsVisible = true; }
        }

        private void AudioUrlRetrievedHandler(string audioUrl)
        {
            //Set Audioplayers endpoint streamUrl.
            Dispatcher.BeginInvoke(() =>  this.Browser.InvokeScript("setAudioUrl", audioUrl));
        }

    }
}