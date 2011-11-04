using System;
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
using PhoneGuitarTab.Search.UltimateGuitar;
using PhoneGuitarTab.UI.Notation.Infrastructure;

namespace PhoneGuitarTab.UI.Notation.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly InteractionRequest<Notification> submitNotificationInteractionRequest;

        public SettingsViewModel()
        {
            submitNotificationInteractionRequest = new InteractionRequest<Notification>();

            SearchCommand =
                new DelegateCommand(
                    () =>
                        {
                            new NavigationServiceEx().NavigateTo(PageType.Search);
                        });

            CollectionCommand =
                new DelegateCommand(
                    () =>
                        {
                            new NavigationServiceEx().NavigateTo(PageType.Collection);
                        });
            HomeCommand =
                new DelegateCommand(
                    () =>
                        {
                            new NavigationServiceEx().NavigateTo(PageType.Startup);
                        });

            Login =
                new DelegateCommand(
                    () =>
                    {
                        //UgSession.Instance.LogIn();
                    });
          
            if(!IsolatedStorageHelper.IsExistParameter("Settings.Search.IsGuitarPro"))
                IsGuitarPro = IsPowerTab = IsText = true;

            Update();

        }

      

        public override void Update()
        {
            IsGuitarPro = IsolatedStorageHelper.LoadParameter<bool>("Settings.Search.IsGuitarPro");
            IsPowerTab = IsolatedStorageHelper.LoadParameter<bool>("Settings.Search.IsPowerTab");
            IsText = IsolatedStorageHelper.LoadParameter<bool>("Settings.Search.IsText");
        }

        private bool _isGuitarPro;
        public bool IsGuitarPro
        {
            set
            {
                _isGuitarPro = value;
                IsolatedStorageHelper.SaveParameter("Settings.Search.IsGuitarPro", _isGuitarPro);
                RaisePropertyChanged(() => IsGuitarPro);
            }
            get
            {
                return _isGuitarPro;
            }
        }

        private bool _isPowerTab;
        public bool IsPowerTab
        {
            set
            {
                _isPowerTab = value;
                IsolatedStorageHelper.SaveParameter("Settings.Search.IsPowerTab", _isPowerTab);
                RaisePropertyChanged(() => IsPowerTab);
            }
            get
            {
                return _isPowerTab;
            }
        }

        private bool _isText;
        public bool IsText
        {
            set
            {
                _isText = value;
                IsolatedStorageHelper.SaveParameter("Settings.Search.IsText", _isText);
                RaisePropertyChanged(() => IsText);
            }
            get
            {
                return _isText;
            }
        }

        #region Properties
        public IInteractionRequest SubmitNotificationInteractionRequest
        {
            get
            {
                return this.submitNotificationInteractionRequest;
            }
        }

        public DelegateCommand Login
        {
            get;
            private set;
        }

        public DelegateCommand SearchCommand
        {
            get;
            set;
        }

        public DelegateCommand CollectionCommand
        {
            get;
            set;
        }

        public DelegateCommand HomeCommand
        {
            get;
            set;
        }
        #endregion
    }
}
