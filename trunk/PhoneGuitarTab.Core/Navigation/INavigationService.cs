using System;
using System.Collections.Generic;
using System.Windows.Navigation;


namespace PhoneGuitarTab.Core.Navigation
{
    public interface INavigationService
    {
        event NavigatingCancelEventHandler Navigating;
        void NavigateTo(Uri pageUri);
        void NavigateTo(Uri pageUri, Dictionary<string, object> parameters);
        void NavigateTo(IPageType type);
        void NavigateTo(IPageType type, Dictionary<string, object> parameters);
        void GoBack();
    }
}
