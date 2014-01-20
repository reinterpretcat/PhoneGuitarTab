using System;

namespace PhoneGuitarTab.UI.ViewModel
{
    using System.Windows;
    using System.Collections.Generic;
    using PhoneGuitarTab.Core.Dependencies;
    using PhoneGuitarTab.Data;
    using PhoneGuitarTab.UI.Infrastructure;
    using PhoneGuitarTab.UI.Entities;
    using System.Linq;
    using Microsoft.Phone.Shell;
    
    public abstract class TabViewModelBase:  DataContextViewModel
    {
        public string TabContent { get; set; }
       
        protected Tab Tablature { get; set; }
       
        protected IDialogController Dialog { get; set; }

        [Dependency]
        protected TabViewModelBase(IDataContextService database, MessageHub hub)
            : base(database, hub)
        {
            
        }

        protected override void ReadNavigationParameters()
        {
            Tablature = (Tab)NavigationParameters["Tab"];

            Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        Tablature.LastOpened = DateTime.Now;
                        Database.SubmitChanges();
                    });

        }

        public override void LoadStateFrom(IDictionary<string, object> state)
        {
            base.LoadStateFrom(state);
            PhoneApplicationService phoneAppService = PhoneApplicationService.Current;   
        }

        public override void SaveStateTo(IDictionary<string, object> state)
        {
            PhoneApplicationService phoneAppService = PhoneApplicationService.Current;
            phoneAppService.UserIdleDetectionMode = IdleDetectionMode.Enabled;

            base.SaveStateTo(state);
        }

        protected override void DataBind()
        {
          
        }
        
        public void PinTabToStart()
        {
            TilesForTabs.PinTabToStart(Tablature);
        } 
        
        
    }
}
