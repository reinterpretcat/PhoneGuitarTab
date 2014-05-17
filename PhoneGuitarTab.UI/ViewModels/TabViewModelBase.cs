using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Search.SoundCloud;
using PhoneGuitarTab.UI.Data;
using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.Infrastructure;
using PhoneGuitarTab.UI.Resources;

namespace PhoneGuitarTab.UI.ViewModels
{
    public abstract class TabViewModelBase : DataContextViewModel
    {
        public string TabContent { get; set; }

        public Tab Tablature { get; set; }

        public RatingService RatingService { get; private set; }

        protected IDialogController Dialog { get; set; }

        public event AudioUrlRetrievedHandler AudioUrlRetrieved;

        public delegate void AudioUrlRetrievedHandler(string audioUrl);

        [Dependency]
        protected TabViewModelBase(IDataContextService database, RatingService ratingService, MessageHub hub)
            : base(database, hub)
        {
            RatingService = ratingService;
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
        }

        public void Browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            var browser = sender as WebBrowser;
            if (e.Value.StartsWith("onStreamUrlRetrieved"))
            {
                var soundCloudSearch = new SoundCloudSearch((string) browser.InvokeScript("getTrackUrl"));
                soundCloudSearch.SearchCompleted += SoundCloudSearchCompleted;
                soundCloudSearch.Run();
            }
        }

        public void PinTabToStart()
        {
            TilesForTabs.PinTabToStart(Tablature);
        }

        public void StopAudioPlayer(WebBrowser browser)
        {
            System.Windows.Application.Current.RootVisual.Dispatcher.BeginInvoke(
                () => browser.InvokeScript("stopAudioPlayer"));
        }

        #region helpers

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

        #endregion

        #region event handlers

        private void SoundCloudSearchCompleted(object sender)
        {
            var result = sender as SoundCloudSearch;
            AudioUrlRetrieved(result.AudioStreamEndPointUrl);
        }

        #endregion
    }
}