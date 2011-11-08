using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GalaSoft.MvvmLight.Command;
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class StartupViewModel : PhoneGuitarTab.Core.ViewModel
    {
        public StartupViewModel()
        {
            GoTo = new RelayCommand<string>(DoGoTo);
            GoToTabView = new RelayCommand<object>(DoGoToTabView);


            IDataContextService database = Container.Resolve<IDataContextService>();

            TabsHistory = (from Tab t in database.Tabs
                           //where t.LastOpened.HasValue && t.LastOpened.Value !=DateTime.MinValue
                           orderby t.LastOpened descending 
                           select t).Take(4).ToList();
            //detect version
            string name = typeof(App).Assembly.FullName;
            AssemblyName asmName = new AssemblyName(name);
            ProductVersion = asmName.Version.ToString();
        }

        #region GoTo command

        public RelayCommand<string> GoTo
        {
            get;
            private set;
        }

        private void DoGoTo(string pageName)
        {
            //Note remove this
            try
            {
                navigationService.NavigateTo(PageType.Get(pageName));
            }
            catch
            {
                System.Windows.MessageBox.Show(String.Format("Under construction: {0}", pageName));
            }
        }

        public RelayCommand<object> GoToTabView
        {
            get;
            private set;
        }

        private void DoGoToTabView(object args)
        {
            var selector = (args as System.Windows.Controls.SelectionChangedEventArgs);
            Tab tab = selector.AddedItems[0] as Tab;
            navigationService.NavigateTo(PageType.Get(PageType.EnumType.TextTab), new Dictionary<string, object>()
                                                                        {
                                                                            {"Tab", tab}
                                                                        });
        }

        #endregion

        public string ProductVersion { get; set; }
        public List<Tab> TabsHistory { get; set; }

        /*  public StartupViewModel()
        {
            GoTo = new DelegateCommand<string>(DoGoTo, CanGoTo);
            ProductVersion = App.ProductVersion;
            TabsHistory = RepositoryHelper.GetAllTabs()
                .Where(t => t.LastOpened != DateTime.MinValue)
                .OrderByDescending(t => t.LastOpened)
                .Take(4).ToList();


        }

        private List<Tab> _tabsHistory;
        public List<Tab> TabsHistory
        {
            set
            {
                _tabsHistory = value;
                RaisePropertyChanged(() => TabsHistory);
            }
            get
            {
                return _tabsHistory;
            }
        }

        private Tab _selectedTab;
        public Tab SelectedTab
        {
            set
            {
                _selectedTab = value;
                RaisePropertyChanged(() => SelectedTab);
                //NOTE: go to specific group page 
                //Commad would be better
                NavigationServiceEx nav = new NavigationServiceEx();
                nav.NavigateToWithParams(PageMapping<ViewModelBase>
                    .GetPageTypeByTab(_selectedTab), new Dictionary<string, object>()
                                                           {
                                                               {"TabId", _selectedTab.Id}
                                                           });
            }
            get { return _selectedTab; }
        }

        public DelegateCommand<string> GoTo
        {
            get;
            private set;
        }

        private void DoGoTo(string arg)
        {
            PageType pageType = (PageType) Enum.Parse(typeof (PageType), arg, true);
            NavigationServiceEx nav = new NavigationServiceEx();
            nav.NavigateTo(pageType);
        }

       

        private bool CanGoTo(string arg)
        {
            return true;
        }*/


        #region Tombstoning

        public override void LoadStateFrom(IDictionary<string, object> state)
        {

        }

        public override void SaveStateTo(IDictionary<string, object> state)
        {

        }

        #endregion

    }
}
