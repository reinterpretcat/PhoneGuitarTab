using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using PhoneGuitarTab.UI.Notation.Infrastructure;
using PhoneGuitarTab.UI.Notation.Persistence;

namespace PhoneGuitarTab.UI.Notation.ViewModel
{
    public class HelpViewModel : ViewModelBase
    {
        //private readonly InteractionRequest<Notification> submitNotificationInteractionRequest;



        public HelpViewModel()
        {

            //submitNotificationInteractionRequest = new InteractionRequest<Notification>();

            //SearchCommand =
            //    new DelegateCommand(
            //        () =>
            //            {
            //                new NavigationServiceEx().NavigateTo(PageType.Search);
            //            });

            //SettingsCommand =
            //    new DelegateCommand(
            //        () =>
            //            {
            //                new NavigationServiceEx().NavigateTo(PageType.Settings);
            //            });
            //HomeCommand =
            //    new DelegateCommand(
            //        () =>
            //            {
            //                new NavigationServiceEx().NavigateTo(PageType.Startup);
            //            });

            Version = App.ProductVersion;

        }




        private string _version;
        public string Version
        {
            get { return _version; }
            set
            {
                if (_version == value)
                    return;
                _version = value;

                RaisePropertyChanged(() => Version);
            }
        }


        #region Properties

        //public IInteractionRequest SubmitNotificationInteractionRequest
        //{
        //    get
        //    {
        //        return this.submitNotificationInteractionRequest;
        //    }
        //}

        //public DelegateCommand SearchCommand
        //{
        //    get;
        //    set;
        //}

        //public DelegateCommand SettingsCommand
        //{
        //    get;
        //    set;
        //}

        //public DelegateCommand HomeCommand
        //{
        //    get;
        //    set;
        //}
        #endregion

        protected override void ReadNavigationParameters()
        {
            //int id;
            //if (Int32.TryParse(NavigationParameters["GroupId"].ToString(), out id))
            //{


           // }
        }


        #region Private

        

        #endregion
    }
}
