using System;
using PhoneGuitarTab.Core.Services;
using PhoneGuitarTab.Search.SoundCloud;
namespace PhoneGuitarTab.UI.ViewModel
{
    using System.Windows;
    using System.Collections.Generic;
    using PhoneGuitarTab.Core.Dependencies;
    using PhoneGuitarTab.Data;
    using PhoneGuitarTab.UI.Infrastructure;
    using PhoneGuitarTab.UI.Entities;
    using PhoneGuitarTab.UI.Data;
    using System.Windows.Navigation;
    using System.Linq;
    using Microsoft.Phone.Shell;
    using Microsoft.Phone.Tasks;
    using System.Net.NetworkInformation;
    using Microsoft.Phone.Controls;

    public abstract class TabViewModelBase:  DataContextViewModel
    {
        public string TabContent { get; set; }
       
        public Tab Tablature { get; set; }

        public RatingService RatingService { get; private set; }

        protected IDialogController Dialog { get; set; }

        private string AudioUrl { get; set; }
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
            Tablature = (Tab)NavigationParameters["Tab"];

            Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        Tablature.LastOpened = DateTime.Now;
                        Database.SubmitChanges();
                    });

        }


        public override void LoadStateFrom(IDictionary<string, object> state)
        {
            base.LoadStateFrom(state);
            PhoneApplicationService phoneAppService = PhoneApplicationService.Current;  // ??? 
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
        
        public void PinTabToStart()
        {
            TilesForTabs.PinTabToStart(Tablature);
        }

        public void Browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            var browser = sender as WebBrowser;
            this.GetAudioStreamUrl(browser);

            this.RunRating();
        }

        public void Browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            var browser = sender as WebBrowser;
            if (e.Value.StartsWith("onStreamUrlRetrieved"))
            {
                SoundCloudSearch soundCloudSearch = new SoundCloudSearch((string)browser.InvokeScript("getTrackUrl"));
                soundCloudSearch.SearchCompleted += SoundCloudSearchCompleted;
                soundCloudSearch.Run();
            }
        }

        #region helpers
       
        private void GetAudioStreamUrl(WebBrowser browser)
        {

            if (NetworkInterface.GetIsNetworkAvailable())
            {
                System.Windows.Application.Current.RootVisual.Dispatcher.BeginInvoke(() =>
                {
                    browser.InvokeScript("getAudioStreamUrl", Tablature.Group.Name + " " + Tablature.Name);

                });
            }
            else
                browser.InvokeScript("setLabel", "Connect your device to internet to stream ", Tablature.Name);
        }
       
        private void RunRating()
        {
            if (!RatingService.IsAppRated() && RatingService.IsNeedShowMessage())
            {
                MessageBoxResult result = MessageBox.Show(AppResources.ReviewTheApp, AppResources.RateTheApp, MessageBoxButton.OKCancel);
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
            this.AudioUrl = result.AudioStreamEndPointUrl;
            this.AudioUrlRetrieved(this.AudioUrl);
        }

        #endregion
      
      
    }
}
