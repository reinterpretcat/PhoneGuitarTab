namespace PhoneGuitarTab.Core.Views
{
    using Microsoft.Phone.Controls;

    using System.Windows.Navigation;

    /// <summary>
    /// PhoneApplicationPage with supporting of tombstoning
    /// </summary>
    public class ViewPage : PhoneApplicationPage
    {
        bool _isNewPageInstance = false;

        // Constructor
        public ViewPage()
        {
            this._isNewPageInstance = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // If this is a back navigation, the page will be discarded, so there
            // is no need to save state.
            //if (e.NavigationMode != NavigationMode.Back)
           // {
                // Save view model state in the page's State dictionary.
                (this.DataContext as ViewModel).SaveStateTo(this.State);
           // }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // If _isNewPageInstance is true, the page constuctor has been called, so
            // state may need to be restored
           // if (_isNewPageInstance)
           // {
                // restore page state
                //if (State.Count > 0)
                    (this.DataContext as ViewModel).LoadStateFrom(this.State);
            //}

            // Set _isNewPageInstance to false. If the user navigates back to this page
            // and it has remained in memory, this value will continue to be false.
            //_isNewPageInstance = false;
        }
    }
}
