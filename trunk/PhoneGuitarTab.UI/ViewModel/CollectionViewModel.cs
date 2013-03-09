using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.UI.Controls;
using PhoneGuitarTab.UI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class CollectionViewModel : DataContextViewModel
    {
        #region Constructor

        public CollectionViewModel()
        {
            CreateCommands();
            DataBind();
            DispatcherHelper.Initialize();
        }

        #endregion Constructor


        #region Properties

        public Tuple<int, Group> SelectedGroup { get; set; }
        public Tab SelectedTab { get; set; }
        public BandByName Groups { get; set; }
        public TabByName AllTabs { get; set; }

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
            AllTabs = new TabByName();
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
                navigationService.NavigateTo(PageType.Get(PageType.EnumType.Group),
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
                navigationService.NavigateTo(PageType.Get(PageType.EnumType.TextTab), new Dictionary<string, object>()
                                                                                          {
                                                                                              {"Tab", tab}
                                                                                          });
            }
        }

        private void DoRemoveTab(int id)
        {
            TabDataContextHelper.DeleteTabById(id);
            DataBind();
        }

        #endregion Command handlers


        #region Helper methods

        private void CreateCommands()
        {
            SearchCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(PageType.EnumType.Search)));
            SettingsCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(PageType.EnumType.Settings)));
            HomeCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(PageType.EnumType.Startup)));

            RemoveTab = new RelayCommand<int>(DoRemoveTab);
            CancelTab = new RelayCommand(() => { });

            GoToGroup = new RelayCommand<object>(DoGoToGroup);
            GoToTabView = new RelayCommand<object>(DoGoToTabView);
        }

        #endregion Helper methods
    }
}
