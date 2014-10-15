using System;
using System.Windows.Input;
using Microsoft.Phone.Tasks;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Views;
using PhoneGuitarTab.Core.Views.Commands;
using PhoneGuitarTab.UI.Infrastructure;
using PhoneGuitarTab.UI.Resources;

namespace PhoneGuitarTab.UI.ViewModels
{
    public class AboutViewModel : ViewModel
    {
        [Dependency]
        public AboutViewModel(PopUpMessageService ratingService)
        {
            AppTitle = AppResources.About_AppTitle;
            CompanyUrl = AppResources.About_CompanyUrl;
            FacebookUrl = AppResources.About_FacebookUrl;
            Copyright = AppResources.About_Copyright;
            SupportEmail = AppResources.About_SupportEmail;
            Support = AppResources.About_Support;
            ApplicationVersion = App.Version;
            _ratingService = ratingService;
        }

        public string CompanyUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string Copyright { get; set; }
        public string SupportEmail { get; set; }
        public string Support { get; set; }
        public string AppTitle { get; set; }
        public string ApplicationVersion { get; set; }

        private readonly PopUpMessageService _ratingService;

        public ICommand ViewWebsiteCommand
        {
            get
            {
                return new ExecuteCommand<string>(url =>
                    new WebBrowserTask {Uri = new Uri(url)}.Show());
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
                _ratingService.RateApp();

                return new ExecuteCommand(() =>
                    new MarketplaceReviewTask().Show());
            }
        }
    }
}