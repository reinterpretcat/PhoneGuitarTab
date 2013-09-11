﻿
using PhoneGuitarTab.Data;
using PhoneGuitarTab.UI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PhoneGuitarTab.UI.ViewModel
{
    using PhoneGuitarTab.Core.Dependencies;
    using PhoneGuitarTab.Core.Primitives;
    using PhoneGuitarTab.Core.Views.Commands;

    public class CollectionViewModel : DataContextViewModel
    {
        #region Fields

        private BandByName groups;
        private TabsByName allTabs;
        private bool isPendingChangesOnCollection = false;

        #endregion Fields


        #region Constructor

        [Dependency]
        public CollectionViewModel(IDataContextService database)
            : base(database)
        {
            CreateCommands();
            DataBind();

            RegisterMessages();

            //DispatcherHelper.Initialize();
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

        public ExecuteCommand<object> GoToGroup
        {
            get;
            private set;
        }

        public ExecuteCommand<object> GoToTabView
        {
            get;
            private set;
        }

        public ExecuteCommand<int> RemoveTab
        {
            get;
            private set;
        }

        public ExecuteCommand SearchCommand
        {
            get;
            set;
        }

        public ExecuteCommand SettingsCommand
        {
            get;
            set;
        }

        public ExecuteCommand HomeCommand
        {
            get;
            set;
        }

        public ExecuteCommand RefreshData
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
            AllTabs = new TabsByName(Database);            
            Groups = new BandByName(Database);
        }

        #endregion Override methods


        #region Command handlers

        private void DoGoToGroup(object sender)
        {
            var selector = sender as Microsoft.Phone.Controls.LongListSelector;
            if (selector != null && selector.SelectedItem is ObservableTuple<int, Group>)
            {
                Group group = (selector.SelectedItem as ObservableTuple<int, Group>).Item2;
                NavigationService.NavigateTo(Strings.Group,
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
                NavigationService.NavigateTo(Strings.TextTab, new Dictionary<string, object>()
                                                                                          {
                                                                                              {"Tab", tab}
                                                                                          });
                selector.SelectedItem = null;
            }
        }

        private void DoRemoveTab(int id)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => Database.DeleteTabById(id));

            RemoveTabFromList(id);
            //MessengerInstance.Send<CollectionTabRemovedMessage>(new CollectionTabRemovedMessage(id));
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
            SearchCommand = new ExecuteCommand(() => NavigationService.NavigateTo(Strings.Search));
            HomeCommand = new ExecuteCommand(() => NavigationService.NavigateTo(Strings.Startup));

            RemoveTab = new ExecuteCommand<int>(DoRemoveTab);

            GoToGroup = new ExecuteCommand<object>(DoGoToGroup);
            GoToTabView = new ExecuteCommand<object>(DoGoToTabView);

            RefreshData = new ExecuteCommand(DoRefreshData);
        }

        private void RegisterMessages()
        {
           // MessengerInstance.Register<HistoryTabRemovedMessage>(this, (message) => { RemoveTabFromList(message.Id); });
           // MessengerInstance.Register<GroupTabRemovedMessage>(this, (message) => { RemoveTabFromList(message.Id); });
           // MessengerInstance.Register<TabsDownloadedMessage>(this, (message) => { isPendingChangesOnCollection = true; });
           // MessengerInstance.Register<RefreshTabsMessage>(this, (message) => { DoRefreshData(); });
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
            //DispatcherHelper.CheckBeginInvokeOnUI(new Action(() => DataBind()));
        }

        #endregion Helper methods
    }
}
