
using System.Windows.Input;
using Microsoft.Phone.Tasks;

namespace PhoneGuitarTab.UI.ViewModel
{
    using PhoneGuitarTab.Core.Views;
    using PhoneGuitarTab.Core.Views.Commands;

    public class AboutViewModel: ViewModel
    {

        public AboutViewModel()
        {
            Help = AppResources.Help;
            AppTitle = AppResources.AppTitle;
            CompanyUrl = AppResources.CompanyUrl;
            Copyright = AppResources.Copyright;
            SupportEmail = AppResources.SupportEmail;
            Support = AppResources.Support;
            SupportMessage = AppResources.SupportMessage;
            ApplicationVersion = App.Version;
        }

        public string Help { get; set; }
        public string CompanyUrl { get; set; }
        public string Copyright { get; set; }
        public string SupportEmail { get; set; }
        public string Support { get; set; }
        public string SupportMessage { get; set; }
        public string AppTitle { get; set; }
        public string ApplicationVersion { get; set; }

        public ICommand ViewWebsiteCommand
        {
            get
            {
                return new ExecuteCommand(() =>
                  new WebBrowserTask { URL = CompanyUrl }.Show());
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
    }
}
