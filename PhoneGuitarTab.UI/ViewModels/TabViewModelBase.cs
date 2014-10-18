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
using PhoneGuitarTab.Search.Audio;
using PhoneGuitarTab.UI.Data;
using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.Infrastructure;
using PhoneGuitarTab.UI.Resources;

namespace PhoneGuitarTab.UI.ViewModels
{
    public abstract class TabViewModelBase : DataContextViewModel
    {
        private readonly IAudioSearcherFactory _audioSearcherFactory;
        private readonly ConfigService _configService;
      
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

        public PopUpMessageService PopUpMessageService { get; private set; }

        private bool isAdEnabled;
        public bool IsAdEnabled
        {
            get
            {
                return isAdEnabled;
            }
        }
        

        [Dependency]
        protected IDialogController Dialog { get; set; }

        public event AudioUrlRetrievedHandler AudioUrlRetrieved;

        public delegate void AudioUrlRetrievedHandler(string audioUrl);

        [Dependency]
        protected TabViewModelBase(IAudioSearcherFactory audioSearcherFactory, IDataContextService database, PopUpMessageService popUpMessageService, ConfigService configService, MessageHub hub)
            : base(database, hub)
        {
            _audioSearcherFactory = audioSearcherFactory;
            _configService = configService;
            PopUpMessageService = popUpMessageService;
            CreateCommands();
            SetConfigVariables();
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
            
            RunPopUpMessages();
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
        }

        public void Browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
          
            if (e.Value.StartsWith("onStreamUrlRetrieved"))
            {
                var onlineAudioSearcher = _audioSearcherFactory.CreateOnlineSearcher();
                onlineAudioSearcher.SearchCompleted += AudioSearchCompleted;
                onlineAudioSearcher.Run((string)Browser.InvokeScript("getTrackUrl"));
            }
        }


        public void StopAudioPlayer()
        {
           Application.Current.RootVisual.Dispatcher.BeginInvoke(
                () => Browser.InvokeScript("stopAudioPlayer"));
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

        /// <summary>
        /// Invokes the browser script to get the start point stream url for the song; note that this url is not the end point.
        /// </summary>
        public void GetOnlineAudioStreamUrl()
        {
            //TODO: Get Local song at first
            if (NetworkInterface.GetIsNetworkAvailable())
            {
               Application.Current.RootVisual.Dispatcher.BeginInvoke(
                    () => Browser.InvokeScript("getAudioStreamUrl", Tablature.Group.Name , Tablature.Name));
            }
            else
                Browser.InvokeScript("setLabel", AppResources.Tab_ConnectDevicePhraseStart,
                    AppResources.Tab_ConnectDevicePhraseEnd);
        }

        private void RunPopUpMessages()
        {
            //Run if ads are enabled only
            if (IsAdEnabled)
            {
                // Run the App rate or review
                if (!PopUpMessageService.IsAppRated() && PopUpMessageService.IsNeedShowRatingMessage())
                {
                    MessageBoxResult result = MessageBox.Show(AppResources.Tab_ReviewTheApp, AppResources.Tab_RateTheApp,
                        MessageBoxButton.OKCancel);
                    //show message.
                    if (result == MessageBoxResult.OK)
                    {
                        PopUpMessageService.RateApp();
                        new MarketplaceReviewTask().Show();
                    }
                }

                //Ask for purchasing the pro - a one time question.
                if (PopUpMessageService.IsNeedShowPurchaseProMessage())
                {
                    MessageBoxResult result = MessageBox.Show(AppResources.Tab_PurchaseProText, AppResources.Tab_PurchaseProHeader,
                       MessageBoxButton.OKCancel);

                    if (result == MessageBoxResult.OK)
                    {
                        MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
                        //Change the app GUID to the pro version
                        marketplaceDetailTask.ContentIdentifier = "00a9816d-8059-4ebb-aff7-277f0b5366b1";
                        marketplaceDetailTask.ContentType = MarketplaceContentType.Applications;

                        marketplaceDetailTask.Show();
                    }
                  
                }
            }
          
        }

        private void CreateCommands()
        {
            PinToStart = new ExecuteCommand(DoPinToStart);
            ToggleSlide = new ExecuteCommand<object>(DoToggleSlide);
            ToggleLightMode = new ExecuteCommand(DoToggleLightMode);
        }

        private void SetConfigVariables()
        {
            isAdEnabled = _configService.AdEnabled;
        }


        #endregion

        #region event handlers

        private void AudioSearchCompleted(object sender)
        {
            var searcher = sender as IAudioSearcher;
            AudioUrlRetrieved(searcher.AudioStreamEndPointUrl);
        }

        #endregion
    }
}