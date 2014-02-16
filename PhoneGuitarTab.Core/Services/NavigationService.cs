

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Diagnostic;
using PhoneGuitarTab.Core.Views;

namespace PhoneGuitarTab.Core.Services
{
    /// <summary>
    /// Default implementation of navigation service
    /// </summary>
    public class NavigationService : INavigationService
    {
        [Dependency]
        public IContainer Container { get; set; }

        [Dependency]
        public ITrace Trace { get; set; }

        [Dependency]
        public TraceCategory Category { get; set; }

        private PhoneApplicationFrame _mainFrame;
        public event NavigatingCancelEventHandler Navigating;

        private readonly Dictionary<string, Uri> _uriMapping = new Dictionary<string, Uri>();
        private readonly Dictionary<string, Primitives.Lazy<ViewModel>> _viewModelMapping = new Dictionary<string, Primitives.Lazy<ViewModel>>();

        public NavigationService(Dictionary<string, Uri> uriMapping, Dictionary<string, Primitives.Lazy<ViewModel>> viewModelMapping)
        {
            _uriMapping = uriMapping;
            _viewModelMapping = viewModelMapping;
        }

        /// <summary>
        /// Navigates to the specific Uri
        /// </summary>
        /// <param name="pageUri"></param>
        public void NavigateTo(Uri pageUri)
        {
            if (EnsureMainFrame())
            {
                Trace.Info(Category, String.Format("Navigate to {0}", pageUri));
                _mainFrame.Navigate(pageUri);
            }
        }

        /// <summary>
        /// Navigates to the specific Uri with parameters
        /// </summary>
        /// <param name="pageUri"></param>
        /// <param name="parameters"></param>
        public void NavigateTo(Uri pageUri, Dictionary<string, object> parameters)
        {
            string page = _uriMapping.Keys.Single(m => _uriMapping[m] == pageUri);
            NavigateTo(page, parameters);
        }

        /// <summary>
        /// Navigates to the specific page
        /// </summary>
        /// <param name="name"></param>
        public void NavigateTo(string name)
        {
            Uri pageUri = _uriMapping[name];
            NavigateTo(pageUri);
        }

        /// <summary>
        /// Navigates to the specific page with parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        public void NavigateTo(string name, Dictionary<string, object> parameters)
        {
            ViewModel vm = _viewModelMapping[name].Value;
            vm.NavigationParameters = parameters;
            NavigateTo(name);
        }

        public void GoBack()
        {
            if (EnsureMainFrame() && _mainFrame.CanGoBack)
            {
                Trace.Info(Category, String.Format("Go back from {0}", _mainFrame.CurrentSource));
                _mainFrame.GoBack();
            }
        }

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


    }
}
