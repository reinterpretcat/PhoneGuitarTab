using System.Windows.Input;
using Microsoft.Phone.Tasks;
using System;
namespace PhoneGuitarTab.UI.ViewModel
{
    using PhoneGuitarTab.Core.Views;
    using PhoneGuitarTab.Core.Views.Commands;
    using PhoneGuitarTab.UI.Data;
    public class AboutViewModel: ViewModel
    {

        public AboutViewModel()
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
            _appSettings = new AppSettings();
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

        private AppSettings _appSettings; 

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
                if (_appSettings.AddOrUpdateValue(AppSettings.isAppRatedKeyName, true))
                    _appSettings.Save();

                return new ExecuteCommand(() =>
                  new MarketplaceReviewTask().Show());
            }
        }
    }
}
