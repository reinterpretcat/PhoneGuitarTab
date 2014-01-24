using Microsoft.Phone.Tasks;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.UI.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Shell;
using System;
namespace PhoneGuitarTab.UI.ViewModel
{
    using PhoneGuitarTab.Core.Dependencies;
    using PhoneGuitarTab.Core.Views.Commands;
    using PhoneGuitarTab.UI.Infrastructure;

    public class StartupViewModel : DataContextViewModel
    {
        #region Fields

        private TabsForHistory _tabsHistory;

        #endregion Fields


        #region Constructors

        [Dependency]
        public StartupViewModel(IDataContextService database, MessageHub hub)
            : base(database, hub)
        {
            CreateCommands();

           // MessengerInstance.Register<CollectionTabRemovedMessage>(this, (message) => { RemoveTabFromList(message.Id); });
          //  MessengerInstance.Register<GroupTabRemovedMessage>(this, (message) => { RemoveTabFromList(message.Id); });

            ProductVersion = App.Version;
          
        }

        #endregion Constructors


        #region Properties

        public string ProductVersion { get; set; }

        public TabsForHistory TabsHistory
        {
            get { return _tabsHistory; }
            set
            {
                _tabsHistory = value;
                RaisePropertyChanged("TabsHistory");
            }
        }

        #endregion Properties

        
        #region Commands

        public ExecuteCommand<string> GoTo
        {
            get;
            private set;
        }

        public ExecuteCommand<object> GoToTabView
        {
            get;
            private set;
        }

       /* public ExecuteCommand Review
        {
            get;
            private set;
        }*/

        public ExecuteCommand<int> RemoveTab
        {
            get;
            private set;
        }

       /* public ExecuteCommand CancelTab
        {
            get;
            private set;
        }*/

        #endregion Commands


        #region Command handlers

        private void DoGoTo(string pageName)
        {
            NavigationService.NavigateTo(pageName);
        }

        private void DoGoToTabView(object args)
        {
            var tabEntity = (args as TabEntity);
            if (tabEntity != null)
            {
                Tab tab = (from Tab t in Database.Tabs
                           where t.Id == tabEntity.Id
                           select t).Single();
                NavigationService.NavigateToTab(new Dictionary<string, object>() { { "Tab", tab } });
            }
        }

        //private void DoReview()
        //{
        //    new MarketplaceReviewTask().Show();
        //}

        private void DoRemoveTab(int id)
        {
            RemoveTabFromList(id);
            Deployment.Current.Dispatcher.BeginInvoke(() => Database.DeleteTabById(id));
            Hub.RaiseHistoryTabRemoved(id);
        }

        #endregion Command handlers


        #region Override members

        protected override void DataBind()
        {
            TabsHistory = new TabsForHistory(5, Database);
        }

        public override void LoadStateFrom(IDictionary<string, object> state)
        {
            base.LoadStateFrom(state);
            PhoneApplicationService phoneAppService = PhoneApplicationService.Current;
            phoneAppService.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            if (state.ContainsKey("TabUrl"))
            {
                String tabUrl = (String)state["TabUrl"];
                string[] parsed = tabUrl.Split('?');
                if (parsed.Length > 1) //If Url contains a tab ID array lenght will be 2.
                {
                    var tabId = System.Convert.ToInt16(parsed[1]);
                    NavigationService.NavigateToTab(new Dictionary<string, object>() { { "Tab", this.Database.GetTabById(tabId) } });

                }

            }

        }

        public override void SaveStateTo(IDictionary<string, object> state)
        {
            PhoneApplicationService phoneAppService = PhoneApplicationService.Current;
            phoneAppService.UserIdleDetectionMode = IdleDetectionMode.Enabled;

            base.SaveStateTo(state);
        }

        #endregion Override members


        #region Helper methods

        private void CreateCommands()
        {
            GoTo = new ExecuteCommand<string>(DoGoTo);
            GoToTabView = new ExecuteCommand<object>(DoGoToTabView);
            //Review = new ExecuteCommand(DoReview);
            RemoveTab = new ExecuteCommand<int>(DoRemoveTab);
        }

        private void RemoveTabFromList(int id)
        {
            var tabToDelete = TabsHistory.Tabs.Where(tab => tab.Id == id).FirstOrDefault();
            if (tabToDelete != null)
            {
                TabsHistory.Tabs.Remove(tabToDelete);
            }
        }

        #endregion Helper methods
    }
}
