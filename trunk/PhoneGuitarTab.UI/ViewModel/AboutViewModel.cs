
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class AboutViewModel: Core.ViewModel
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
                return new RelayCommand(() =>
                  new WebBrowserTask { URL = CompanyUrl }.Show());
            }
        }

        public ICommand SupportQuestionCommand
        {
            get
            {
                return new RelayCommand(() =>
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
