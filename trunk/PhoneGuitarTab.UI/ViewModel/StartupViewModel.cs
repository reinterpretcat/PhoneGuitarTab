using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.Infrastructure;
using PhoneGuitarTab.UI.Infrastructure.Enums;
using PhoneGuitarTab.UI.Infrastructure.Messages;
using System.Collections.Generic;
using System.Linq;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class StartupViewModel : DataContextViewModel
    {
        #region Fields

        private TabsForHistory _tabsHistory;

        #endregion Fields


        #region Constructors

        public StartupViewModel()
        {
            CreateCommands();

            MessengerInstance.Register<CollectionTabRemovedMessage>(this, (message) => { RemoveTabFromList(message.Id); });
            MessengerInstance.Register<GroupTabRemovedMessage>(this, (message) => { RemoveTabFromList(message.Id); });

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

        public RelayCommand<string> GoTo
        {
            get;
            private set;
        }

        public RelayCommand<object> GoToTabView
        {
            get;
            private set;
        }

        public RelayCommand Review
        {
            get;
            private set;
        }

        public RelayCommand<int> RemoveTab
        {
            get;
            private set;
        }

        public RelayCommand CancelTab
        {
            get;
            private set;
        }

        #endregion Commands


        #region Command handlers

        private void DoGoTo(string pageName)
        {
            navigationService.NavigateTo(PageType.Get(pageName));
        }

        private void DoGoToTabView(object args)
        {
            var tabEntity = (args as TabEntity);
            if (tabEntity != null)
            {
                Tab tab = (from Tab t in Database.Tabs
                           where t.Id == tabEntity.Id
                           select t).Single();
                navigationService.NavigateTo(PageType.Get(ViewType.TextTab), new Dictionary<string, object>()
                                                                                          {
                                                                                              {"Tab", tab}
                                                                                          });
            }
        }

        private void DoReview()
        {
            new MarketplaceReviewTask().Show();
        }

        private void DoRemoveTab(int id)
        {
            TabDataContextHelper.DeleteTabById(id);

            RemoveTabFromList(id);

            MessengerInstance.Send<HistoryTabRemovedMessage>(new HistoryTabRemovedMessage(id));
        }

        #endregion Command handlers


        #region Override members

        protected override void DataBind()
        {
            TabsHistory = new TabsForHistory(5);
        }

        public override void LoadStateFrom(IDictionary<string, object> state)
        {
            base.LoadStateFrom(state);
        }

        public override void SaveStateTo(IDictionary<string, object> state)
        {
            base.SaveStateTo(state);
        }

        #endregion Override members


        #region Helper methods

        private void CreateCommands()
        {
            GoTo = new RelayCommand<string>(DoGoTo);
            GoToTabView = new RelayCommand<object>(DoGoToTabView);
            Review = new RelayCommand(DoReview);
            RemoveTab = new RelayCommand<int>(DoRemoveTab);
        }

        private void RemoveTabFromList(int id)
        {
            //var tabToDelete = TabsHistory.Tabs.Where(tab => tab.Id == id).FirstOrDefault();
            //if (tabToDelete != null)
            //{
            //    TabsHistory.Tabs.Remove(tabToDelete);
            //    RaisePropertyChanged("TabsHistory");
            //}

            DataBind();
        }

        #endregion Helper methods
    }
}
