using System;
using PhoneGuitarTab.Core.Services;

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
    using System.Net;
    using System.IO;
    using Microsoft.Phone.Controls;

    public abstract class TabViewModelBase:  DataContextViewModel
    {
        public string TabContent { get; set; }
       
        public Tab Tablature { get; set; }

        public RatingService RatingService { get; private set; }

        protected IDialogController Dialog { get; set; }

        public event AudioUrlRetrievedHandler AudioUrlRetrieved;
        public delegate void AudioUrlRetrievedHandler();

        public string AudioUrl { get; set; }
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
            this.RunRating();
        }


        private void RunRating()
        {
            if (!RatingService.IsAppRated() && RatingService.IsNeedShowMessage())
            {
                MessageBoxResult result = MessageBox.Show( AppResources.ReviewTheApp, AppResources.RateTheApp, MessageBoxButton.OKCancel);
                //show message.
                if (result == MessageBoxResult.OK)
                {
                    RatingService.RateApp();
                    new MarketplaceReviewTask().Show();
                }
            }
        }

        public void GetTrackStreamRedirectUrl(string sourceUrl)
        {
            if (!string.IsNullOrEmpty(sourceUrl))
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(sourceUrl + ".json?client_id=5ca9c93662aaa8d953a421ce53500bae");
                request.Method = "HEAD";
                request.AllowReadStreamBuffering = true;
                request.AllowAutoRedirect = true;
                request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallback), request);
            }
        }

        private void ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult);


            using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
            {
                this.AudioUrl = myResponse.ResponseUri.AbsoluteUri;
                AudioUrlRetrieved();

            }
            myResponse.Close();

        }

       
    }
}
