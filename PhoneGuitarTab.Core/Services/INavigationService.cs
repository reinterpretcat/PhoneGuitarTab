using System;
using System.Collections.Generic;
using System.Windows.Navigation;

namespace PhoneGuitarTab.Core.Services
{
    /// <summary>
    ///     Represents navigation service
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        ///     Navigating event
        /// </summary>
        event NavigatingCancelEventHandler Navigating;

        /// <summary>
        ///     Navigates to uri
        /// </summary>
        void NavigateTo(Uri pageUri);

        /// <summary>
        ///     Navigates to uri with parameters
        /// </summary>
        void NavigateTo(Uri pageUri, Dictionary<string, object> parameters);

        /// <summary>
        ///     Navigates to page specified
        /// </summary>
        void NavigateTo(string name);

        /// <summary>
        ///     Navigates to page with parameters
        /// </summary>
        void NavigateTo(string name, Dictionary<string, object> parameters);

        /// <summary>
        ///     Goes back
        /// </summary>
        void GoBack();
    }
}