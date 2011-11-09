using System;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class HelpViewModel: Core.ViewModel
    {
     
        public HelpViewModel()
        {
            Help = AppResources.Help;
            AppTitle = AppResources.AppTitle;
            CompanyUrl = AppResources.CompanyUrl;
            Copyright = AppResources.Copyright;
            SupportEmail = AppResources.SupportEmail;
            Support = AppResources.Support;
            SupportMessage = AppResources.SupportMessage;
            ApplicationVersion = App.Version;
            Review = AppResources.Review;
            ReviewTheApp = AppResources.ReviewTheApp;
        }

        public string Help { get; set; }
        public string CompanyUrl { get; set; }
        public string Copyright { get; set; }
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
