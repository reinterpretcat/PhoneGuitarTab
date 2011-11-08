using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.UI.Infrastructure;


namespace PhoneGuitarTab.UI.ViewModel
{
    public class GroupViewModel : PhoneGuitarTab.Core.ViewModel
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

        public Group CurrentGroup { get; set; }
        public List<Tab> Tabs { get; set; }

        #region Actions

        private void DoGoToTabView(object args)
        {
            var selector = (args as System.Windows.Controls.SelectionChangedEventArgs);
            Tab tab = selector.AddedItems[0] as Tab;
            navigationService.NavigateTo(PageType.Get(PageType.EnumType.TextTab), new Dictionary<string, object>()
                                                                        {
                                                                            {"Tab", tab}
                                                                        });
        }

        private void DoRemoveTab(int id)
        {
            TabDataContextHelper.DeleteTabById(id);
            UpdateTabs();
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

        private void UpdateTabs()
        {
            IDataContextService database = Container.Resolve<IDataContextService>();
            Tabs = (from Tab t in database.Tabs
                    where t.Group.Id == CurrentGroup.Id
                    select t).ToList();
            
            RaisePropertyChanged("Tabs");
        }

        protected override void ReadNavigationParameters()
        {

            if (NavigationParameters == null)
                return;

            CurrentGroup = (Group)NavigationParameters["group"];
            UpdateTabs();
        }
    }
}
