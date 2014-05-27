using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneGuitarTab.Core.Views;
using PhoneGuitarTab.UI.Resources;
using PhoneGuitarTab.UI.ViewModels;

namespace PhoneGuitarTab.UI.Views
{
    // TODO move code-behind to viewmodel
    public partial class StaveTabView : ViewPage
    {
        //Generic List for ListPicker data binding.
        public ObservableCollection<StaveTabViewModel.Track> Tracks;
        public bool _isNewPageInstance = false;
        //flag to prevent renavigation of browser while switching back and forth from listpicker's fullmode
        private bool isFirstLoad = true;

        private bool isBrowserReady;
        // Url of Home page
        private const string SandboxUri = "/Html/sandbox.html";

        public StaveTabView()
        {
            InitializeComponent();
            _isNewPageInstance = true;
            //Generic list that keeps track data - binded to the listpicker.
            Tracks = new ObservableCollection<StaveTabViewModel.Track>();
            ListPickerInstrument.DataContext = Tracks;

            //set slider's Browser property to current.
            slider.Browser = Browser;

            var viewModel = DataContext as StaveTabViewModel;
            viewModel.Browser = Browser;

            //subscribe to AudioURLretrieved event.
            viewModel.AudioUrlRetrieved += AudioUrlRetrievedHandler;
            viewModel.PropertyChanged += viewModel_PropertyChanged;
        }

        void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           //Note that Bindable Application Bar does not support binding with Element name.

            if (e.PropertyName == "AutoScrollToggled")
            {
                this.ToggleSlider();
            }

            if (e.PropertyName == "ScaleRequested")
            {
                this.ScaleTabs();
            }

            if (e.PropertyName == "InstrumentsDemanded")
            {
                this.ShowInstruments();
            }
        }

        private void Browser_Loaded(object sender, RoutedEventArgs e)
        {
            Browser.IsScriptEnabled = true;

            //Navigate only for the first load (to prevent renavigation while switching back and forth from listpicker's fullmode)
            if (isFirstLoad)
            {
                Browser.Navigate(new Uri(SandboxUri, UriKind.Relative));
            }
        }

        private void ScaleTabs()
        {
            Browser.InvokeScript("scaleChange");
            if (ApplicationBar.Mode != ApplicationBarMode.Minimized)
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
        }

        ////Appbar button to visible / hide for listpicker.
        private void ShowInstruments()
        {
            ListPickerInstrument.Open();

            if (ApplicationBar.Mode != ApplicationBarMode.Minimized)
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
        }

        //Event that is fired after picking an instrument (latebinding after ListPicker populated)
        private void ListPickerInstrument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Browser.InvokeScript("changeInstrument", ListPickerInstrument.SelectedIndex.ToString());
        }

        // Handle navigation failures.
        private void Browser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            MessageBox.Show(AppResources.Stave_NavigationFailed);
        }

        private void Browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            var viewModel = DataContext as StaveTabViewModel;
            if (e.Value.StartsWith("onReady"))
            {
                Browser.InvokeScript("pullTabContent", viewModel.TabContent, viewModel.IsNightMode.ToString());
                OrientationChanged += (_, __) => Browser.InvokeScript("showTab");
            }
            isBrowserReady = true;
            viewModel.Browser_ScriptNotify(sender, e);
        }

        private void Browser_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Autoscroll - Tap to slide/stop.
            if (slider.Visibility == Visibility.Visible)
            {
                if (slider.isScrolling)
                    slider.stopAutoScroll(sender);
                else
                    slider.invokeAutoScroll(sender);
            }

            //Collapse ListPicker.
            ListPickerInstrument.Visibility = Visibility.Collapsed;

            if (ApplicationBar.Mode != ApplicationBarMode.Minimized)
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
        }

        /// <summary>
        ///     Helper Method to set the progress indicator.
        /// </summary>
        /// <param name="isVisible"></param>
        /// <param name="indicatorText"></param>
        private void SetProgressIndicator(bool isVisible, [Optional] string indicatorText)
        {
            if (((TransitionFrame) System.Windows.Application.Current.RootVisual).Content == this)
            {
                SystemTray.IsVisible = isVisible;

                SystemTray.ProgressIndicator.IsIndeterminate = isVisible;
                SystemTray.ProgressIndicator.IsVisible = isVisible;
                if (!String.IsNullOrEmpty(indicatorText))
                    SystemTray.ProgressIndicator.Text = indicatorText;
            }
        }

        //Helper method to populate the list with track data.
        private void FillTrackList(ObservableCollection<StaveTabViewModel.Track> trackList)
        {
            string trackLenght = (string) Browser.InvokeScript("getTrackCount");

            //for some strange reason track count returns as multiplied by 2 from JS side. Therefore here is a division by 2 for counter.
            for (int i = 0; i < Convert.ToInt16(trackLenght)/2; i++)
            {
                trackList.Add(
                    new StaveTabViewModel.Track((string) Browser.InvokeScript("getInstrumentName", i.ToString()), i, ""));
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
                SetProgressIndicator(true, AppResources.Stave_Loading);
            }
        }

        private void Browser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //stop progress indicator
            SetProgressIndicator(false);

            //Fill the tracklist just once in the first load.
            if (isFirstLoad & ((TransitionFrame) System.Windows.Application.Current.RootVisual).Content == this)
            {
                FillTrackList(Tracks);
                ListPickerInstrument.SelectionChanged += ListPickerInstrument_SelectionChanged;
                ListPickerInstrument.Visibility = Visibility.Visible;
                slider.MouseLeftButtonUp += Slider_Clicked;
                isFirstLoad = false;

                var viewModel = DataContext as StaveTabViewModel;
                viewModel.Browser_LoadCompleted(sender, e);
            }
        }

        private void Slider_Clicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Hide listpicker when slider is clicked.
            ListPickerInstrument.Visibility = Visibility.Collapsed;
        }

        private void ToggleSlider()
        {
            //Control Autoscroll behaviour
            if (slider.Visibility == Visibility.Visible)
            {
                slider.Visibility = Visibility.Collapsed;
                slider.stopAutoScroll(slider.Browser);
            }
            else
            {
                slider.Visibility = Visibility.Visible;
                slider.invokeAutoScroll(slider.Browser);
            }

            if (ApplicationBar.Mode != ApplicationBarMode.Minimized)
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var viewModel = DataContext as StaveTabViewModel;
            viewModel.RatingService.IncreaseTabViewCount();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Enabled;
            if (isBrowserReady)
            {
                var viewModel = DataContext as StaveTabViewModel;
                viewModel.StopAudioPlayer(Browser);
            }
        }

        private void ViewPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            //Control the appbar visibility (in landscape mode it opens the buttons, reading tabs become harder)
            if ((Orientation == PageOrientation.LandscapeLeft) ||
                (Orientation == PageOrientation.LandscapeRight))
            {
                ApplicationBar.IsVisible = false;
            }
            else
            {
                ApplicationBar.IsVisible = true;
            }
        }

        private void AudioUrlRetrievedHandler(string audioUrl)
        {
            //Set Audioplayers endpoint streamUrl.
            Dispatcher.BeginInvoke(() => Browser.InvokeScript("setAudioUrl", audioUrl));
        }
    }
}