using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Views.Commands;
using PhoneGuitarTab.Search;
using PhoneGuitarTab.Search.SoundCloud;
using PhoneGuitarTab.UI.Data;
using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.Infrastructure;
using PhoneGuitarTab.UI.Resources;

namespace PhoneGuitarTab.UI.ViewModels
{
    public abstract class TabViewModelBase : DataContextViewModel
    {
        private readonly IAudioSearcher _audioSearcher;
        public string TabContent { get; set; }

        public Tab Tablature { get; set; }

        public WebBrowser Browser { get; set; }

        public bool IsNightMode { get; private set; }

        private bool _autoScrollToggled;

        public bool AutoScrollToggled
        {
            get { return _autoScrollToggled; }
            set
            {
                _autoScrollToggled = value;
                RaisePropertyChanged("AutoScrollToggled");
            }
        }

        public RatingService RatingService { get; private set; }

        [Dependency]
        protected IDialogController Dialog { get; set; }

        public event AudioUrlRetrievedHandler AudioUrlRetrieved;

        public delegate void AudioUrlRetrievedHandler(string audioUrl);

        [Dependency]
        protected TabViewModelBase(IAudioSearcher audioSearcher, IDataContextService database, RatingService ratingService, MessageHub hub)
            : base(database, hub)
        {
            _audioSearcher = audioSearcher;
            RatingService = ratingService;
            CreateCommands();
        }


        protected override void ReadNavigationParameters()
        {
            Tablature = (Tab) NavigationParameters["Tab"];

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Tablature.LastOpened = DateTime.Now;
                Database.SubmitChanges();
            });
        }

        public override void SaveStateTo(IDictionary<string, object> state)
        {
            PhoneApplicationService phoneAppService = PhoneApplicationService.Current;
            phoneAppService.UserIdleDetectionMode = IdleDetectionMode.Enabled;

            base.SaveStateTo(state);
        }

        protected override void DataBind()
        {
        }

        public void Browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            var browser = sender as WebBrowser;
            GetAudioStreamUrl(browser);
            RunRating();
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
        }

        public void Browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            var browser = sender as WebBrowser;
            if (e.Value.StartsWith("onStreamUrlRetrieved"))
            {              
                _audioSearcher.SearchCompleted += AudioSearchCompleted;
                _audioSearcher.Run((string)browser.InvokeScript("getTrackUrl"));
            }
        }


        public void StopAudioPlayer(WebBrowser browser)
        {
            System.Windows.Application.Current.RootVisual.Dispatcher.BeginInvoke(
                () => browser.InvokeScript("stopAudioPlayer"));
        }

        #region Commands

        public ExecuteCommand<object> ToggleSlide { get; private set; }

        public ExecuteCommand PinToStart { get; private set; }

        public ExecuteCommand ToggleLightMode { get; private set; }

        #endregion Commands

        #region Command handlers

        private void DoPinToStart()
        {
            TilesForTabs.PinTabToStart(Tablature);
        }

        private void DoToggleSlide(object arg)
        {
            AutoScrollToggled = true;
        }

        private void DoToggleLightMode()
        {
            IsNightMode = !IsNightMode;
            Browser.InvokeScript("toggleLightMode", IsNightMode.ToString());
        }

        #endregion Command handlers

        #region Helpers

        private void GetAudioStreamUrl(WebBrowser browser)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                System.Windows.Application.Current.RootVisual.Dispatcher.BeginInvoke(
                    () => browser.InvokeScript("getAudioStreamUrl", Tablature.Group.Name + " " + Tablature.Name));
            }
            else
                browser.InvokeScript("setLabel", AppResources.Tab_ConnectDevicePhraseStart,
                    AppResources.Tab_ConnectDevicePhraseEnd);
        }

        private void RunRating()
        {
            if (!RatingService.IsAppRated() && RatingService.IsNeedShowMessage())
            {
                MessageBoxResult result = MessageBox.Show(AppResources.Tab_ReviewTheApp, AppResources.Tab_RateTheApp,
                    MessageBoxButton.OKCancel);
                //show message.
                if (result == MessageBoxResult.OK)
                {
                    RatingService.RateApp();
                    new MarketplaceReviewTask().Show();
                }
            }
        }

        private void CreateCommands()
        {
            PinToStart = new ExecuteCommand(DoPinToStart);
            ToggleSlide = new ExecuteCommand<object>(DoToggleSlide);
            ToggleLightMode = new ExecuteCommand(DoToggleLightMode);
        }

        #endregion

        #region event handlers

        private void AudioSearchCompleted()
        {
            
            AudioUrlRetrieved(_audioSearcher.AudioStreamEndPointUrl);
        }

        #endregion
    }
}