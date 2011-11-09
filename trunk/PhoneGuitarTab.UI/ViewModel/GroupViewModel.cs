using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.UI.Infrastructure;


namespace PhoneGuitarTab.UI.ViewModel
{
    public class GroupViewModel : DataContextViewModel
    {
        public GroupViewModel()
        {

            SearchCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(PageType.EnumType.Search)));
            SettingsCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(PageType.EnumType.Settings)));
            HomeCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(PageType.EnumType.Startup)));

            GoToTabView = new RelayCommand<object>(DoGoToTabView);
            RemoveTab = new RelayCommand<int>(DoRemoveTab);
            CancelTab = new RelayCommand(() => { });

        }

        protected override void DataBind()
        {
            Tabs = (from Tab t in Database.Tabs
                    orderby t.Name ascending 
                    where t.Group.Id == CurrentGroup.Id
                    select t).ToList();
        }

        public Group _currentGroup;
        public Group CurrentGroup
        {
            get { return _currentGroup; }
            set
            {
                _currentGroup = value;              
                RaisePropertyChanged("CurrentGroup");
                DataBind();
            }
        }

        public List<Tab> _tabs;
        public List<Tab> Tabs
        {
            get { return _tabs; }
            set
            {
                _tabs = value;
                RaisePropertyChanged("Tabs");
            }
        }

        #region Actions

        private void DoGoToTabView(object args)
        {
            var selector = (args as System.Windows.Controls.SelectionChangedEventArgs);
            if (selector != null && selector.AddedItems.Count > 0 && selector.AddedItems[0] is Tab)
            {
                Tab tab = selector.AddedItems[0] as Tab;
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

        #endregion

        #region Commands

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

        public RelayCommand<object> GoToTabView
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

        #endregion
   
        protected override void ReadNavigationParameters()
        {
            if (NavigationParameters == null)
                return;
            CurrentGroup = (Group)NavigationParameters["group"];
        }

        public override void SaveStateTo(IDictionary<string, object> state)
        {
            base.SaveStateTo(state);
            state["CurrentGroupId"] = CurrentGroup.Id;
        }

        public override void LoadStateFrom(IDictionary<string, object> state)
        {
            base.LoadStateFrom(state);
            if (state.ContainsKey("CurrentGroupId"))
            {
                int currentGroupId = (int) state["CurrentGroupId"];
            }

        }
    }
}
