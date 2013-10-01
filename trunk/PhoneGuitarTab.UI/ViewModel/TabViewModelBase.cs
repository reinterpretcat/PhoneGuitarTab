using System;

namespace PhoneGuitarTab.UI.ViewModel
{
    using System.Windows;

    using Microsoft.Phone.Shell;

    using PhoneGuitarTab.Core.Dependencies;
    using PhoneGuitarTab.Data;
    using PhoneGuitarTab.UI.Infrastructure;

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

        public override void LoadStateFrom(System.Collections.Generic.IDictionary<string, object> state)
        {
            PhoneApplicationService phoneAppService = PhoneApplicationService.Current;
            phoneAppService.UserIdleDetectionMode = IdleDetectionMode.Disabled;

            base.LoadStateFrom(state);
        }

        public override void SaveStateTo(System.Collections.Generic.IDictionary<string, object> state)
        {
            PhoneApplicationService phoneAppService = PhoneApplicationService.Current;
            phoneAppService.UserIdleDetectionMode = IdleDetectionMode.Enabled;

            base.SaveStateTo(state);
        }
    }
}
