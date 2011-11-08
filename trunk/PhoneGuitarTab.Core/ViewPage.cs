using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace PhoneGuitarTab.Core
{
    /// <summary>
    /// PhoneApplicationPage with supporting of tombstoning
    /// </summary>
    public class ViewPage : PhoneApplicationPage
    {
        bool _isNewPageInstance = false;

        // Constructor
        public ViewPage()
        {
            _isNewPageInstance = true;
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // If this is a back navigation, the page will be discarded, so there
            // is no need to save state.
            //if (e.NavigationMode != NavigationMode.Back)
           // {
                // Save view model state in the page's State dictionary.
                (DataContext as ViewModel).SaveStateTo(State);
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
                    (DataContext as ViewModel).LoadStateFrom(State);
            //}

            // Set _isNewPageInstance to false. If the user navigates back to this page
            // and it has remained in memory, this value will continue to be false.
            //_isNewPageInstance = false;
        }
    }
}
