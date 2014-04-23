
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
    using PhoneGuitarTab.UI.Infrastructure;

    public class CollectionViewModel : DataContextViewModel
    {
        #region Fields

        private BandByName groups;
        private TabsByName allTabs;
        private bool isPendingChangesOnCollection = false;
        private bool isSelectionEnabled;
        private string backGroundImage;
        private List<int> _selectedItemIds;
        #endregion Fields

        #region Constructor

        [Dependency]
        public CollectionViewModel(IDataContextService database, MessageHub hub)
            : base(database, hub)
        {
            CreateCommands();
            RegisterEvents();
            DataBind();
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
        public List<int> SelectedItemIds
        {
            get
            {
                return _selectedItemIds;
            }
            set
            {
                _selectedItemIds = value;
                RaisePropertyChanged("SelectedItemIds");
            }
        }

        public bool IsSelectionEnabled
        {
            get
            {
                return isSelectionEnabled;
            }
            set
            {
                isSelectionEnabled = value;
                RaisePropertyChanged("IsSelectionEnabled");
                Hub.RaiseSelectorIsSelectionEnabled(isSelectionEnabled);
              
            }
        }

        public string BackGroundImage
        {
            get
            {
                return backGroundImage;
            }
            set
            {
                backGroundImage = value;
                RaisePropertyChanged("BackGroundImage");
            }
        }
        #endregion Properties


        #region Commands

        public ExecuteCommand<object> GoToGroup
        {
            get;
            private set;
        }

        public ExecuteCommand<object> SetSelectedItems
        {
            get;
            private set;
        }
      
  
        public ExecuteCommand<object> SetIsSelectionEnabled
        {
            get;
            private set;
        }

        public ExecuteCommand<object> GoToTabView
        {
            get;
            private set;
        }

        public ExecuteCommand<int> PinTabToStart
        {
            get;
            private set;
        }

        public ExecuteCommand<int> RemoveTab
        {
            get;
            private set;
        }

        public ExecuteCommand RemoveTabs
        {
            get;
            private set;
        }

        public ExecuteCommand SearchCommand
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

        protected override void DataBind()
        {
            AllTabs = new TabsByName(Database);            
            Groups = new BandByName(Database);
            SelectedItemIds = new List<int>();
        }

        #endregion Override methods


        #region Command handlers

        private void DoGoToGroup(object sender)
        {
            var selector = sender as Microsoft.Phone.Controls.LongListSelector;
            if (selector != null && selector.SelectedItem is ObservableTuple<int, Group>)
            {
                Group group = (selector.SelectedItem as ObservableTuple<int, Group>).Item2;
                this.BackGroundImage = group.ExtraLargeImageUrl;
                NavigationService.NavigateTo(Strings.Group, new Dictionary<string, object> { { "group", group } });
            }
        }

        private void DoGoToTabView(object sender)
        {
            TabEntity tabEntity = sender as TabEntity;
            if (tabEntity != null)
            {
               
                Tab tab = (from Tab t in Database.Tabs
                           where t.Id == tabEntity.Id
                           select t).Single();
                NavigationService.NavigateToTab(new Dictionary<string, object>() { { "Tab", tab } });
            }
        }
      

        private void DoRemoveTab(int id)
        {
            TilesForTabs.RemoveTabFromStart(Database.GetTabById(id));
            Deployment.Current.Dispatcher.BeginInvoke(() => Database.DeleteTabById(id));
            RemoveTabFromList(id);
            Hub.RaiseCollectionTabRemoved(id);
        }
        private void DoRemoveTabs()
        {
            foreach (int id in SelectedItemIds)
            {
                this.DoRemoveTab(id);   
            }
     
            this.SelectedItemIds.Clear();
        }

        private void DoPinTabToStart(int id)
        {
            
            Tab tab = (from Tab t in Database.Tabs
                       where t.Id == id
                       select t).Single();

            TilesForTabs.PinTabToStart(tab);
        }

        private void DoSetIsSelectionEnabled(object arg)
        {
            
            if (arg.GetType() == typeof( Microsoft.Phone.Controls.LongListMultiSelector))
            {
                var selector = arg as Microsoft.Phone.Controls.LongListMultiSelector;
                this.IsSelectionEnabled = selector.IsSelectionEnabled;
            }
            else if(arg.ToString() == "true" || arg.ToString() =="false")
            {

                this.IsSelectionEnabled = Convert.ToBoolean(arg.ToString()); ;
            }                    
        }
        private void DoSetSelectedItems(object sender)        
        {
            this.SelectedItemIds.Clear();
            var selector = sender as Microsoft.Phone.Controls.LongListMultiSelector;
            foreach (TabEntity item in selector.SelectedItems)
            {
                this.SelectedItemIds.Add(item.Id);
            }
        }

        private void DoRefreshData()
        {
            if (!isPendingChangesOnCollection)
                return;
            Deployment.Current.Dispatcher.BeginInvoke(
            () =>
            {
                DataBind();
                isPendingChangesOnCollection = false;
            });
        }

        #endregion Command handlers


        #region Helper methods

        private void CreateCommands()
        {
            SearchCommand = new ExecuteCommand(() => NavigationService.NavigateTo(Strings.Search));
          

            RemoveTab = new ExecuteCommand<int>(DoRemoveTab);
            RemoveTabs = new ExecuteCommand(DoRemoveTabs);
            PinTabToStart = new ExecuteCommand<int>(DoPinTabToStart);
            GoToGroup = new ExecuteCommand<object>(DoGoToGroup);
            GoToTabView = new ExecuteCommand<object>(DoGoToTabView);
            SetIsSelectionEnabled = new ExecuteCommand<object>(DoSetIsSelectionEnabled);
            SetSelectedItems = new ExecuteCommand<object>(DoSetSelectedItems);
            RefreshData = new ExecuteCommand(DoRefreshData);
        }

        private void RegisterEvents()
        {
            Hub.HistoryTabRemoved += (o, id) => RemoveTabFromList(id);
            Hub.GroupTabRemoved += (o, id) => RemoveTabFromList(id);
            Hub.TabsDownloaded += (o, args) => isPendingChangesOnCollection = true;
            Hub.TabsRefreshed += (o, args) => DoRefreshData();
          
        }

        private void RemoveTabFromList(int id)
        {
            var tabToRemove = AllTabs.Tabs.Single(tab => tab.Id == id);
            AllTabs.RemoveTab(tabToRemove);
            Groups.DecreaseTabCount(tabToRemove.Group);
            RaisePropertyChanged("AllTabs");
        }

        #endregion Helper methods
    }
}
