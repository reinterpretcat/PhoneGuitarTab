using System.Windows.Input;
using Microsoft.Phone.Tasks;
using System;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Services;

namespace PhoneGuitarTab.UI.ViewModel
{
    using PhoneGuitarTab.Core.Views;
    using PhoneGuitarTab.Core.Views.Commands;
    using PhoneGuitarTab.UI.Data;
    public class AboutViewModel: ViewModel
    {
        [Dependency]
        public AboutViewModel(ISettingService settingService)
        {
            Help = AppResources.Help;
            AppTitle = AppResources.AppTitle;
            CompanyUrl = AppResources.CompanyUrl;
            FacebookUrl = AppResources.FacebookUrl;
            Copyright = AppResources.Copyright;
            SupportEmail = AppResources.SupportEmail;
            Support = AppResources.Support;
            SupportMessage = AppResources.SupportMessage;
            Review = AppResources.Review;
            ReviewTheApp = AppResources.ReviewTheApp;
            ApplicationVersion = App.Version;
            _settingService = settingService;  
        }
       
        public string Help { get; set; }
        public string CompanyUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string Copyright { get; set; }
        public string SupportEmail { get; set; }
        public string Support { get; set; }
        public string SupportMessage { get; set; }
        public string AppTitle { get; set; }
        public string ApplicationVersion { get; set; }
        public string Review { get; set; }
        public string ReviewTheApp { get; set; }

        private ISettingService _settingService; 

        public ICommand ViewWebsiteCommand
        {
            get
            {
                return new ExecuteCommand<string>( url =>
                  new WebBrowserTask { Uri = new Uri(url) }.Show());
            }
        }

        public ICommand SupportQuestionCommand
        {
            get
            {
                return new ExecuteCommand(() =>
                {
                    var emailComposeTask = new EmailComposeTask
                    {
                        To = SupportEmail,
                        Subject =
                          Support + " " + AppTitle + " " +
                          ApplicationVersion
                    };
                    emailComposeTask.Show();
                });
            }
        }

        public ICommand ReviewCommand
        {
            get
            {
                if (_settingService.AddOrUpdateValue(AppSettingService.isAppRatedKeyName, true))
                    _settingService.Save();

                return new ExecuteCommand(() =>
                  new MarketplaceReviewTask().Show());
            }
        }
    }
}
