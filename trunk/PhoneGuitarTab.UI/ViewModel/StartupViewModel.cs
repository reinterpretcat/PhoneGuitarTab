using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.UI.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class StartupViewModel : DataContextViewModel
    {
        #region Fields

        private List<Tab> _tabsHistory;

        #endregion Fields


        #region Constructors

        public StartupViewModel()
        {
            GoTo = new RelayCommand<string>(DoGoTo);
            GoToTabView = new RelayCommand<object>(DoGoToTabView);
            Review = new RelayCommand(DoReview);

            ProductVersion = App.Version;
        }

        #endregion Constructors


        #region Properties

        public string ProductVersion { get; set; }

        public List<Tab> TabsHistory
        {
            get { return _tabsHistory; }
            set
            {
                _tabsHistory = value;
                RaisePropertyChanged("TabsHistory");
            }
        }

        #endregion Properties

        
        #region Commands

        public RelayCommand<string> GoTo
        {
            get;
            private set;
        }

        public RelayCommand<object> GoToTabView
        {
            get;
            private set;
        }

        public RelayCommand Review
        {
            get;
            private set;
        }

        #endregion Commands


        #region Command handlers

        private void DoGoTo(string pageName)
        {
            navigationService.NavigateTo(PageType.Get(pageName));
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

        private void DoReview()
        {
            new MarketplaceReviewTask().Show();
        }

        #endregion Command handlers


        #region Override members

        protected override void DataBind()
        {
            TabsHistory = (from Tab t in Database.Tabs
                           orderby t.LastOpened descending
                           select t).Take(5).ToList();
        }

        public override void LoadStateFrom(IDictionary<string, object> state)
        {
            base.LoadStateFrom(state);
        }

        public override void SaveStateTo(IDictionary<string, object> state)
        {
            base.SaveStateTo(state);
        }

        #endregion Override members
    }
}
