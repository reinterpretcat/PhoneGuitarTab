using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.Infrastructure;
using PhoneGuitarTab.UI.Infrastructure.Enums;
using PhoneGuitarTab.UI.Infrastructure.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class CollectionViewModel : DataContextViewModel
    {
        #region Fields

        private BandByName groups;
        private TabsByName allTabs;
        private bool isPendingChangesOnCollection = false;

        #endregion Fields


        #region Constructor

        public CollectionViewModel()
        {
            CreateCommands();
            DataBind();

            RegisterMessages();

            DispatcherHelper.Initialize();
        }

        #endregion Constructor


        #region Properties

        public BandByName Groups 
        {
            get
            {
                return groups;
            }
            set
            {
                groups = value;
                RaisePropertyChanged("Groups");
            }
        }

        public TabsByName AllTabs 
        {
            get
            {
                return allTabs;
            }
            set
            {
                allTabs = value;
                RaisePropertyChanged("AllTabs");
            }
        }

        #endregion Properties


        #region Commands

        public RelayCommand<object> GoToGroup
        {
            get;
            private set;
        }

        public RelayCommand<object> GoToTabView
        {
            get;
            private set;
        }

        public RelayCommand<int> RemoveTab
        {
            get;
            private set;
        }

        public RelayCommand SearchCommand
        {
            get;
            set;
        }

        public RelayCommand SettingsCommand
        {
            get;
            set;
        }

        public RelayCommand HomeCommand
        {
            get;
            set;
        }

        public RelayCommand RefreshData
        {
            get;
            set;
        }

        #endregion Commands


        #region Override methods

        public override void SaveStateTo(IDictionary<string, object> state)
        {
            base.SaveStateTo(state);
            
        }

        public override void LoadStateFrom(IDictionary<string, object> state)
        {
           base.LoadStateFrom(state);
        }

        protected override void DataBind()
        {
            AllTabs = new TabsByName();            
            Groups = new BandByName();
        }

        #endregion Override methods


        #region Command handlers

        private void DoGoToGroup(object sender)
        {
            var selector = sender as Microsoft.Phone.Controls.LongListSelector;
            if (selector != null && selector.SelectedItem is ObservableTuple<int, Group>)
            {
                Group group = (selector.SelectedItem as ObservableTuple<int, Group>).Item2;
                navigationService.NavigateTo(PageType.Get(ViewType.Group),
                                             new Dictionary<string, object> { { "group", group } });
            }
        }

        private void DoGoToTabView(object sender)
        {
            Microsoft.Phone.Controls.LongListSelector selector = sender as Microsoft.Phone.Controls.LongListSelector;
            if (selector != null && selector.SelectedItem is TabEntity)
            {
                TabEntity tabEntity = selector.SelectedItem as TabEntity;
                Tab tab = (from Tab t in Database.Tabs
                           where t.Id == tabEntity.Id
                           select t).Single();
                navigationService.NavigateTo(PageType.Get(ViewType.TextTab), new Dictionary<string, object>()
                                                                                          {
                                                                                              {"Tab", tab}
                                                                                          });
                selector.SelectedItem = null;
            }
        }

        private void DoRemoveTab(int id)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => TabDataContextHelper.DeleteTabById(id));

            RemoveTabFromList(id);
            MessengerInstance.Send<CollectionTabRemovedMessage>(new CollectionTabRemovedMessage(id));
        }

        private void DoRefreshData()
        {
            if (!isPendingChangesOnCollection)
                return;
            Deployment.Current.Dispatcher.BeginInvoke(
            () =>
            {
                AddDownloadedTabs();
                isPendingChangesOnCollection = false;
            });
        }

        #endregion Command handlers


        #region Helper methods

        private void CreateCommands()
        {
            SearchCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(ViewType.Search)));
            SettingsCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(ViewType.Settings)));
            HomeCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(ViewType.Startup)));

            RemoveTab = new RelayCommand<int>(DoRemoveTab);

            GoToGroup = new RelayCommand<object>(DoGoToGroup);
            GoToTabView = new RelayCommand<object>(DoGoToTabView);

            RefreshData = new RelayCommand(DoRefreshData);
        }

        private void RegisterMessages()
        {
            MessengerInstance.Register<HistoryTabRemovedMessage>(this, (message) => { RemoveTabFromList(message.Id); });
            MessengerInstance.Register<GroupTabRemovedMessage>(this, (message) => { RemoveTabFromList(message.Id); });
            MessengerInstance.Register<TabsDownloadedMessage>(this, (message) => { isPendingChangesOnCollection = true; });
            MessengerInstance.Register<RefreshTabsMessage>(this, (message) => { DoRefreshData(); });
        }

        private void RemoveTabFromList(int id)
        {
            var tabToRemove = AllTabs.Tabs.Where(tab => tab.Id == id).Single();
            AllTabs.RemoveTab(tabToRemove);
            Groups.DecreaseTabCount(tabToRemove.Group);
            RaisePropertyChanged("AllTabs");
        }

        private void AddDownloadedTabs()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(new Action(() => DataBind()));
        }

        #endregion Helper methods
    }
}
