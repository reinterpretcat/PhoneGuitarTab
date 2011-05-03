using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using PhoneGuitarTab.UI.Notation.ViewModel;

namespace PhoneGuitarTab.UI.Notation.Infrastructure
{
    public class NavigationServiceEx
    {

        public NavigationServiceEx()
        {
            Frame = Application.Current.RootVisual as PhoneApplicationFrame;
            Frame.Navigated += (s, e) =>
                                   {
                                       PhoneApplicationPage view = e.Content as PhoneApplicationPage;

                                       if (view == null)
                                           return;

                                       var viewModel = PageMapping<ViewModelBase>.GetViewModel(e.Uri);

                                       viewModel.Update();
                                       view.DataContext = viewModel;

                                   };
        }

        protected PhoneApplicationFrame Frame
        {
            get;
            set;
        }

        public void NavigateTo(PageType pageType)
        {
           
            Frame.Navigate(PageMapping<ViewModelBase>.GetUri(pageType));
        }

        public void NavigateToWithParams(PageType pageType, Dictionary<string, object> parameters)
        {
            var navigatedViewModel = PageMapping<ViewModelBase>.GetViewModel(pageType);
            navigatedViewModel.NavigationParameters = parameters;
            navigatedViewModel.Update();

            Frame.Navigate(PageMapping<ViewModelBase>.GetUri(pageType));
        }
    }
}
