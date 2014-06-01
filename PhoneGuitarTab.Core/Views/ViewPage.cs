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
           
        }
    }
}