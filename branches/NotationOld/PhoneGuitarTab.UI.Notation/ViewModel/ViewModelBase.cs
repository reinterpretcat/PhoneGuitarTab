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
using Microsoft.Phone.Shell;
using Microsoft.Practices.Prism.ViewModel;
using PhoneGuitarTab.UI.Notation.Infrastructure;

namespace PhoneGuitarTab.UI.Notation.ViewModel
{
    public class ViewModelBase : NotificationObject
    {
        private Dictionary<string, object> navigationParameters = null;

        public virtual void Update(){}

        public Dictionary<string, object> NavigationParameters
        {
            get
            {
                return navigationParameters;
            }
            set
            {
                navigationParameters = value;
                ReadNavigationParameters();
            }
        }

        public ViewModelBase()
        {
            //PhoneApplicationService.Current.Deactivated += (s, e) => IsBeingDeactivated();
            //PhoneApplicationService.Current.Activated += (s, e) => IsBeingActivated();
            //PhoneApplicationService.Current.Launching += (s, e) => IsBeingLaunched();
        }

        public virtual void IsBeingLaunched()
        {
            
        }

        public virtual void IsBeingDeactivated()
        {

        }

        public virtual void IsBeingActivated()
        {
            new NavigationServiceEx().NavigateTo(PageType.Startup);
        }

        protected virtual void ReadNavigationParameters()
        {
        }
    }
}
