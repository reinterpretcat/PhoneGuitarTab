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

    public abstract class TabViewModelBase:  DataContextViewModel
    {
        public string TabContent { get; set; }
       
        protected Tab Tablature { get; set; }

        private ISettingService _settingService;

        protected IDialogController Dialog { get; set; }

        [Dependency]
        protected TabViewModelBase(IDataContextService database, ISettingService settingService, MessageHub hub)
            : base(database, hub)
        {
            _settingService = settingService;   
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
                        if (_settingService.AddOrUpdateValue(AppSettingService.isAppRatedKeyName, true))
                            _settingService.Save();
                        new MarketplaceReviewTask().Show();
                    }
                }
            }
        }
       
        private int GetTabViewCountMod()
        {

            return _settingService.GetValueOrDefault<int>(AppSettingService.tabViewCountKeyName, AppSettingService.tabViewCountDefault) % 4;            
        }

        public void IncreaseTabViewCount()
        {
            int tabCount = _settingService.GetValueOrDefault<int>(AppSettingService.tabViewCountKeyName, AppSettingService.tabViewCountDefault);

            if (this._settingService.AddOrUpdateValue(AppSettingService.tabViewCountKeyName, (tabCount + 1)))
                this._settingService.Save();
        }

        private bool IsAppRated()
        {
            return _settingService.GetValueOrDefault<bool>(AppSettingService.isAppRatedKeyName, AppSettingService.isAppratedDefault);
        }

        
    }
}
