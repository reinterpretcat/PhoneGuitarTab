using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using System.Windows.Input;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class HelpViewModel: Core.ViewModel
    {
     
        public HelpViewModel()
        {
            AppTitle = AppResources.AppTitle;
            SupportEmail = AppResources.SupportEmail;
            Support = AppResources.Support;
            SupportMessage = AppResources.SupportMessage;
            ApplicationVersion = App.Version;
            Review = AppResources.Review;
            ReviewTheApp = AppResources.ReviewTheApp;
        }

        public string Help { get; set; }
        public string SupportEmail { get; set; }
        public string Support { get; set; }
        public string SupportMessage { get; set; }
        public string AppTitle { get; set; }
        public string ApplicationVersion { get; set; }
        public string Review { get; set; }
        public string ReviewTheApp { get; set; }
 
        public ICommand ReviewCommand
        {
            get
            {
                return new RelayCommand(() =>
                  new MarketplaceReviewTask().Show());
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
