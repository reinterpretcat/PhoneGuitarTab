using System;

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

    public abstract class TabViewModelBase:  DataContextViewModel
    {
        public string TabContent { get; set; }
       
        protected Tab Tablature { get; set; }

        private AppSettings _appSettings;
        protected IDialogController Dialog { get; set; }

        [Dependency]
        protected TabViewModelBase(IDataContextService database, MessageHub hub)
            : base(database, hub)
        {
            _appSettings = new AppSettings();    
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
            PhoneApplicationService phoneAppService = PhoneApplicationService.Current;   
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
            if (!IsAppRated())
            {
                if (GetTabViewCountMod() == 0)
                {
                    MessageBoxResult result = MessageBox.Show( AppResources.ReviewTheApp, AppResources.RateTheApp, MessageBoxButton.OKCancel);
                    //show message.
                    if (result == MessageBoxResult.OK)
                    {
                        if (_appSettings.AddOrUpdateValue(AppSettings.isAppRatedKeyName, true))
                            _appSettings.Save();
                        new MarketplaceReviewTask().Show();
                    }
                }
            }
        }
       
        private int GetTabViewCountMod()
        {
          
           return _appSettings.GetValueOrDefault<int>(AppSettings.tabViewCountKeyName, AppSettings.tabViewCountDefault) % 4;            
        }

        public void IncreaseTabViewCount()
        {
            int tabCount = _appSettings.GetValueOrDefault<int>(AppSettings.tabViewCountKeyName, AppSettings.tabViewCountDefault);

            if (this._appSettings.AddOrUpdateValue(AppSettings.tabViewCountKeyName, (tabCount + 1)))
                this._appSettings.Save();
        }

        private bool IsAppRated()
        {
            return _appSettings.GetValueOrDefault<bool>(AppSettings.isAppRatedKeyName, AppSettings.isAppratedDefault);
        }

        
    }
}
