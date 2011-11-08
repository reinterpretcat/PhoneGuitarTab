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
    public class StartupViewModel : DataContextViewModel
    {
        public StartupViewModel()
        {
            GoTo = new RelayCommand<string>(DoGoTo);
            GoToTabView = new RelayCommand<object>(DoGoToTabView);

            //detect version
            string name = typeof(App).Assembly.FullName;
            AssemblyName asmName = new AssemblyName(name);
            ProductVersion = asmName.Version.ToString();
        }

        protected override void DataBind()
        {
            TabsHistory = (from Tab t in Database.Tabs
                           //where t.LastOpened.HasValue && t.LastOpened.Value !=DateTime.MinValue
                           orderby t.LastOpened descending
                           select t).Take(4).ToList();
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
            if (selector != null && selector.AddedItems.Count > 0)
            {
                Tab tab = selector.AddedItems[0] as Tab;
                navigationService.NavigateTo(PageType.Get(PageType.EnumType.TextTab), new Dictionary<string, object>()
                                                                                          {
                                                                                              {"Tab", tab}
                                                                                          });
            }
        }

        #endregion

        public string ProductVersion { get; set; }

        private List<Tab> _tabsHistory;
        public List<Tab> TabsHistory
        {
            get { return _tabsHistory; }
            set
            {
                _tabsHistory = value;
                RaisePropertyChanged("TabsHistory");
            }
        }

        #region Tombstoning

        public override void LoadStateFrom(IDictionary<string, object> state)
        {
            base.LoadStateFrom(state);
        }

        public override void SaveStateTo(IDictionary<string, object> state)
        {
            base.SaveStateTo(state);
        }

        #endregion

    }
}
