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

namespace PhoneGuitarTab.UI.ViewModel
{
    public class CollectionViewModel : DataContextViewModel
    {
        #region Fields

        private BandByName groups;
        private TabsByName allTabs;

        #endregion Fields


        #region Constructor

        public CollectionViewModel()
        {
            CreateCommands();
            DataBind();

            MessengerInstance.Register<HistoryTabRemovedMessage>(this, (message) => { RemoveTabFromList(message.Id); });
            MessengerInstance.Register<GroupTabRemovedMessage>(this, (message) => { RemoveTabFromList(message.Id); });

            DispatcherHelper.Initialize();
        }

        #endregion Constructor


        #region Properties

        public Tuple<int, Group> SelectedGroup { get; set; }

        public Tab SelectedTab { get; set; }

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

        public RelayCommand CancelTab
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
            Groups = new BandByName();
            RaisePropertyChanged("Groups");
            //get songs list
            AllTabs = new TabsByName();
            RaisePropertyChanged("AllTabs");
        }

        #endregion Override methods


        #region Public methods

        public void AddDownloadedTab(Tab newTab)
        {
            //TODO (cent) test performance on large data collection
            // if it will impact ui responsiveness, rewrite
            DispatcherHelper.CheckBeginInvokeOnUI(new Action(() => DataBind())); 
        }

        #endregion Public methods


        #region Command handlers

        private void DoGoToGroup(object sender)
        {
            var selector = sender as Microsoft.Phone.Controls.LongListSelector;
            if (selector != null && selector.SelectedItem is Tuple<int, Group>)
            {
                Group group = (selector.SelectedItem as Tuple<int, Group>).Item2;
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
            TabDataContextHelper.DeleteTabById(id);

            RemoveTabFromList(id);

            MessengerInstance.Send<CollectionTabRemovedMessage>(new CollectionTabRemovedMessage(id));
        }

        #endregion Command handlers


        #region Helper methods

        private void CreateCommands()
        {
            SearchCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(ViewType.Search)));
            SettingsCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(ViewType.Settings)));
            HomeCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(ViewType.Startup)));

            RemoveTab = new RelayCommand<int>(DoRemoveTab);
            CancelTab = new RelayCommand(() => { });

            GoToGroup = new RelayCommand<object>(DoGoToGroup);
            GoToTabView = new RelayCommand<object>(DoGoToTabView);
        }

        private void RemoveTabFromList(int id)
        {
            //AllTabs.RemoveTab(AllTabs.Tabs.Where(tab => tab.Id == id).Single());
            //RaisePropertyChanged("AllTabs");

            // can't realize why RaisePropertyChanged won't work
            // TODO it later, but for now:
            DataBind();
        }

        #endregion Helper methods
    }
}
