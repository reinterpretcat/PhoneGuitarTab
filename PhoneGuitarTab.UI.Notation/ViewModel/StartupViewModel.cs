using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Commands;
using PhoneGuitarTab.UI.Notation.Infrastructure;
using PhoneGuitarTab.UI.Notation.Persistence;

namespace PhoneGuitarTab.UI.Notation.ViewModel
{
    public class StartupViewModel : ViewModelBase
    {

        public StartupViewModel()
        {
            GoTo = new DelegateCommand<string>(DoGoTo, CanGoTo);
            ProductVersion = App.ProductVersion;
            TabsHistory = RepositoryHelper.GetAllTabs()
                .Where(t => t.LastOpened != DateTime.MinValue)
                .OrderByDescending(t => t.LastOpened)
                .Take(4).ToList();


        }

        public override void IsBeingDeactivated()
        {
            base.IsBeingDeactivated();
            //Tombstoning.Save(GetType()+".GoTo", GoTo);
           // Tombstoning.Save(GetType() + ".TabHistory", TabsHistory);
           // Tombstoning.Save(GetType() + ".ProductVersion", ProductVersion);
        }

        public override void IsBeingActivated()
        {
            base.IsBeingActivated();
            //GoTo = Tombstoning.Load<DelegateCommand<string>>(GetType() + ".GoTo");
            //TabsHistory = Tombstoning.Load<List<Tab>>(GetType() + ".TabHistory");
            //ProductVersion = Tombstoning.Load<string>(GetType() + ".ProductVersion");
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

        private string _productVersion;
        public string ProductVersion
        {
            set
            {
                _productVersion = value;
                RaisePropertyChanged(() => ProductVersion);
            }
            get
            {
                return _productVersion;
            }
        }

        private bool CanGoTo(string arg)
        {
            return true;
        }

    }
}
