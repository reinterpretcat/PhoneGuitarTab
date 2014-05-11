using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace PhoneGuitarTab.Core.Views
{
    /// <summary>
    ///     PhoneApplicationPage with supporting of tombstoning
    /// </summary>
    public class ViewPage : PhoneApplicationPage
    {
        public bool _isNewPageInstance = false;

        // Constructor
        public ViewPage()
        {
            _isNewPageInstance = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // If this is a back navigation, the page will be discarded, so there
            // is no need to save state.
            if (e.NavigationMode != NavigationMode.Back)
            {
                // Save view model state in the page's State dictionary.
                (DataContext as ViewModel).SaveStateTo(State);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // If _isNewPageInstance is true, the page constuctor has been called, so
            // state may need to be restored

            if (_isNewPageInstance)
            {
                if (!e.IsNavigationInitiator)
                    State["TabUrl"] = e.Uri.OriginalString;
                // restore page state
                if (State.Count > 0)
                    (DataContext as ViewModel).LoadStateFrom(State);
            }

            // Set _isNewPageInstance to false. If the user navigates back to this page
            // and it has remained in memory, this value will continue to be false.
            _isNewPageInstance = false;
        }
    }
}