using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;

namespace PhoneGuitarTab.Core.Navigation
{
    public class NavigationService : INavigationService
    {
        #region Fields

        private PhoneApplicationFrame _mainFrame;
        private PageMapping _pageMapping;

        #endregion Fields


        #region Constructor

        public NavigationService()
        {
           _pageMapping = Container.Resolve<PageMapping>();
        }

        #endregion Constructor


        #region Events

        public event NavigatingCancelEventHandler Navigating;

        #endregion Events


        #region Public methods

        public void NavigateTo(Uri pageUri)
        {
            if (EnsureMainFrame())
            {
                _mainFrame.Navigate(pageUri);
            }
        }

        public void NavigateTo(IPageType type)
        {
            Uri pageUri = _pageMapping.GetUri(type);
            NavigateTo(pageUri);
        }

        public void NavigateTo(Uri pageUri, Dictionary<string,object> parameters)
        {
            IPageType pageType = _pageMapping.GetPageType(pageUri);
            NavigateTo(pageType, parameters);
        }

        public void NavigateTo(IPageType type, Dictionary<string, object> parameters)
        {
            ViewModel vm = _pageMapping.GetViewModel(type);
            vm.NavigationParameters = parameters;
            NavigateTo(type);
        }

        public void GoBack()
        {
            if (EnsureMainFrame() && _mainFrame.CanGoBack)
            {
                _mainFrame.GoBack();
            }
        }

        #endregion Public methods


        #region Helper methods

        private bool EnsureMainFrame()
        {
            if (_mainFrame != null)
            {
                return true;
            }

            _mainFrame = Application.Current.RootVisual as PhoneApplicationFrame;

            if (_mainFrame != null)
            {
                // Could be null if the app runs inside a design tool
                _mainFrame.Navigating += (s, e) =>
                {
                    if (Navigating != null)
                    {
                        Navigating(s, e);
                    }
                };
                return true;
            }
            return false;
        }

        #endregion Helper methods
    }
}
