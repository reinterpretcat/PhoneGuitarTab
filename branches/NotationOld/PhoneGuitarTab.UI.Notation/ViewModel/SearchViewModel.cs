using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using PhoneGuitarTab.Search;
using PhoneGuitarTab.Search.UltimateGuitar;
using PhoneGuitarTab.UI.Notation.Infrastructure;
using PhoneGuitarTab.UI.Notation.Persistence;

namespace PhoneGuitarTab.UI.Notation.ViewModel
{
    public class SearchViewModel : ViewModelBase
    {
        private SearchTabResult groupSearch;

        private readonly InteractionRequest<Notification> submitNotificationInteractionRequest;

        public SearchViewModel()
        {
            submitNotificationInteractionRequest = new InteractionRequest<Notification>();

            CollectionCommand =
                new DelegateCommand(
                    () =>
                        {
                            new NavigationServiceEx().NavigateTo(PageType.Collection);
                        });

            SettingsCommand =
                new DelegateCommand(
                    () =>
                        {
                            new NavigationServiceEx().NavigateTo(PageType.Settings);
                        });
            HomeCommand =
                new DelegateCommand(
                    () =>
                        {
                            new NavigationServiceEx().NavigateTo(PageType.Startup);
                        });
            LaunchSearch = new DelegateCommand<string>(DoLaunchSearch, CanLaunchSearch);
            SelectPage = new DelegateCommand<string>(DoSelectPage, CanSelectPage);

            DownloadTab = new DelegateCommand<string>(DoDownloadTab, CanDownloadTab);
            CustomDownloadTab = new DelegateCommand<string>(DoCustomDownloadTab, CanCustomDownloadTab);
            CancelTab = new DelegateCommand<string>(DoCancelTab, CanCancelTab);
            SelectedCustomTabType = CustomTabTypes[0];
            //force bindin in order header rendering
            SearchGroupTabs = new TabByName(new List<Tab>());
            SearchGroupTabsSummary = new SearchTabResultSummary();
            CurrentPageIndex = 1;
            CurrentSearchText = String.Empty;
            CustomGroupName = "";
            CustomTabName = "";
            //Pages = Enumerable.Range(1, 1).Select(p=> new Tuple<ICommand,string>(SelectPage,p.ToString())).ToList();
            HeaderPagingVisibility = Visibility.Collapsed;
            Update();
        }


        private bool _isGuitarPro;
        private bool _isPowerTab;
        private bool _isText;

        public override void Update()
        {
            base.Update();
            _isGuitarPro = true; //IsolatedStorageHelper.LoadParameter<bool>("Settings.Search.IsGuitarPro");
            _isPowerTab = IsolatedStorageHelper.LoadParameter<bool>("Settings.Search.IsPowerTab");
            _isText = IsolatedStorageHelper.LoadParameter<bool>("Settings.Search.IsText");
        }

        private bool FilterTab(SearchTabResultEntry entry)
        {
            //TODO optimize this
            /*return ((entry.Type == "guitar pro") && (_isGuitarPro))||
                ((entry.Type == "power tab") && (_isPowerTab))||
                (((entry.Type == "bass") || (entry.Type == "tab")) && (_isText));*/
            return true;
        }

        private void DoLaunchSearch(string arg)
        {
            CurrentSearchText = arg;
            string[] pattern = arg.Split(',');
            string song = pattern.Length > 1 ? pattern[1] : "";
            groupSearch = new SearchTabResult(pattern[0], song);
            groupSearch.SearchComplete += (s, e) =>
                                              {
                                                  //TODO examine e.Error 
                                                  if (e.Error == null)
                                                  {
                                                      var groupTabs = groupSearch.Entries.Where(FilterTab).
                                                          Select(entry => new Tab()
                                                                                          {
                                                                                              SearchId = entry.Id, 
                                                                                              SearchUrl = entry.Url,
                                                                                              Name = entry.Name,
                                                                                              //NOTE do not create group here via repository
                                                                                              Group = new Group(entry.Artist), 
                                                                                              //Path = entry.Url,
                                                                                              Rating = entry.Rating,
                                                                                              Type = entry.Type
                                                                                          }).ToList();
                                                      Deployment.Current.Dispatcher.BeginInvoke(
                                                          () =>
                                                              {
                                                                  SearchGroupTabsSummary = groupSearch.Summary;
                                                                  
                                                                  /*List<Tuple<int,string>> links = new List<Tuple<int, string>>();
                                                                  for (int i = 1; i <= groupSearch.Summary.PageCount; i++)
                                                                      links.Add(new Tuple<int, string>(i, groupSearch.PageLinks.ElementAt(i)));*/
                                                                  Pages = Enumerable.Range(1, groupSearch.Summary.PageCount).Select(p => new Tuple<ICommand, string>(SelectPage, p.ToString())).ToList();
                                                                  HeaderPagingVisibility =
                                                                      groupSearch.Summary.PageCount > 1
                                                                          ? Visibility.Visible
                                                                          : Visibility.Collapsed;
                                                                  SearchGroupTabs = new TabByName(groupTabs);
                                                              });
                                                  }
                                                  IsSearching = false;

                                              };
            IsSearching = true;
            groupSearch.Run(CurrentPageIndex);
            
        }

        private bool CanLaunchSearch(string arg)
        {
            //TODO test arg
            return true;
        }

        private void DoSelectPage(string arg)
        {
            int pageNumber;
            if(Int32.TryParse(arg, out pageNumber))
            {
                CurrentPageIndex = pageNumber;
                DoLaunchSearch(CurrentSearchText);
            }
        }

        private bool CanSelectPage(string arg)
        {
            //TODO test arg
            return true;
        }


        private bool CanCustomDownloadTab(string arg)
        {
            return (!_isSearching) &&
                (!String.IsNullOrEmpty(_customGroupName) &&
                (!String.IsNullOrEmpty(_customTabName)));
        }

        private void DoCustomDownloadTab(string arg)
        {
            if (_isSearching)
            {
                MessageBox.Show("Sorry, you cannot download the tab right now.");
                return;
            }
            string customType = SelectedCustomTabType.Item2;
            string customGroup = CustomGroupName;
            string customTab = CustomTabName;
            string url = arg;
            //
            var filePath = IsolatedStorageHelper.CreateTabFilePath();
            var fileStream = IsolatedStorageHelper.CreateTabFile(filePath);

            FileDownloader downloader = new FileDownloader(fileStream);
            downloader.DownloadComplete += delegate
                                               {
                                                   Tab tab = new Tab()
                                                                 {
                                                                     Id = RepositoryHelper.GetId<Tab>(),
                                                                     Type = customType,
                                                                     Name = customTab
                                                                 };

                tab.Path = filePath;
                //Note get or create new instance from/in database
                tab.Group = RepositoryHelper.GetGroupByName(customGroup);
                RepositoryHelper.Save<Tab>(tab);

                Deployment.Current.Dispatcher.BeginInvoke(
                    () =>
                    {
                        IsSearching = false;
                        MessageBox.Show("Your tab was downloaded");
                    });
            };
            IsSearching = true;

            downloader.Download(url);

        }

        private void DoDownloadTab(string arg)
        {
            if(_isSearching)
            {
                MessageBox.Show("Sorry, you cannot download the tab right now.");
                return;
            }
            var tab =
                SearchGroupTabs.AllTabs.Where(t => t.Id == arg).FirstOrDefault();

            //TODO create converter
            SearchTabResultEntry entry = new SearchTabResultEntry()
                                             {
                                                 Id = tab.SearchId,
                                                 Url = tab.SearchUrl,
                                                 Type = tab.Type
                                             };

            string filePath = IsolatedStorageHelper.CreateTabFilePath();

            //TODO Hard coded credentials
            SearchContext.Instance["name"] = "eiskalt";
            SearchContext.Instance["password"] = "1103897";

            //TODO examine IO errors
            SearchTabDownloader downloader = new SearchTabDownloader(entry, filePath);
            downloader.DownloadComplete += delegate
                                               {
                                                   tab.Id = RepositoryHelper.GetId<Tab>();
                                                   tab.Path = filePath;
                                                   //Note get or create new instance from/in database
                                                   tab.Group = RepositoryHelper.GetGroupByName(tab.Group.Name);
                                                   RepositoryHelper.Save<Tab>(tab);

                                                   Deployment.Current.Dispatcher.BeginInvoke(
                                                       () =>
                                                           {
                                                               IsSearching = false;
                                                               MessageBox.Show("The tab was downloaded");
                                                           });
                                               };
            IsSearching = true;

            try
            {
                if(!UgSession.Instance.IsAuthenticated)
                {
                    UgSession.Instance.BeginLogin("eiskalt", "1103897", ar => downloader.Download(), null);
                }
                else
                {
                    downloader.Download();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error: {0}",ex));
            }


        }

        private bool CanDownloadTab(string arg)
        {
            //TODO test arg
            return !_isSearching;
        }

        private void DoCancelTab(string arg)
        {

        }

        private bool CanCancelTab(string arg)
        {
            return true;
        }

        private string _customGroupName;
        public string CustomGroupName
        {
            get { return _customGroupName; }
            set
            {
                _customGroupName = value;
                RaisePropertyChanged(() => CustomGroupName);
                CustomDownloadTab.RaiseCanExecuteChanged();
            }
        }

        private string _customTabName;
        public string CustomTabName
        {
            get { return _customTabName; }
            set
            {
                _customTabName = value;
                RaisePropertyChanged(() => CustomTabName);
                CustomDownloadTab.RaiseCanExecuteChanged();
            }
        }

        private List<Tuple<string, string>> _customTabTypes;
        public List<Tuple<string,string>> CustomTabTypes
        {
            get
            {
                //create type-image mapping
                if(_customTabTypes == null)
                {
                    _customTabTypes = new List<Tuple<string, string>>();
                    foreach (string key in Tab.ImageTypeMapping.Keys)
                        _customTabTypes.Add(new Tuple<string, string>(key, Tab.ImageTypeMapping[key]));
                }

                return _customTabTypes;
            }
        }

        private Tuple<string, string> _selectedCustomTabType;
        public Tuple<string, string> SelectedCustomTabType
        {
            get { return _selectedCustomTabType; }
            set
            {
                _selectedCustomTabType = value;
                RaisePropertyChanged(() => CustomTabName);
                CustomDownloadTab.RaiseCanExecuteChanged();
            }
        }

        private SearchTabResultSummary _searchGroupTabsSummary;
        public SearchTabResultSummary SearchGroupTabsSummary
        {
            get { return _searchGroupTabsSummary; }
            set
            {
                _searchGroupTabsSummary = value;
                RaisePropertyChanged(() => SearchGroupTabsSummary);
            }
        }

        private Visibility _headerPagingVisibility;
        public Visibility HeaderPagingVisibility
        {
            get { return _headerPagingVisibility; }
            set
            {
                _headerPagingVisibility = value;
                RaisePropertyChanged(() => HeaderPagingVisibility);
            }
        }

        //NOTE I cannot bind Command in ItemsControl with custom ItemPanelTemplate
        //So this type is used instead
        private List<Tuple<ICommand, string>> _pages;
        public List<Tuple<ICommand,string>> Pages
        {
            get { return _pages; }
            set
            {
                _pages = value;
                RaisePropertyChanged(() => Pages);
            }
        }

        private TabByName _searchGroupTabs;
        public TabByName SearchGroupTabs
        {
            set
            {
                _searchGroupTabs = value;
                RaisePropertyChanged(() => SearchGroupTabs);
            }
            get { return _searchGroupTabs; }

        }

        private bool _isSearching;
        public bool IsSearching
        {
            set
            {
                _isSearching = value;
                RaisePropertyChanged(() => IsSearching);
            }
            get { return _isSearching; }
        }

        //helpers properties
        public string CurrentSearchText { get; set; }
        public int CurrentPageIndex { get; set; }

        #region Properties
        public IInteractionRequest SubmitNotificationInteractionRequest
        {
            get
            {
                return this.submitNotificationInteractionRequest;
            }
        }

        public DelegateCommand<string> LaunchSearch
        {
            get;
            private set;
        }

        public DelegateCommand<string> SelectPage
        {
            get;
            private set;
        }

        public DelegateCommand<string> DownloadTab
        {
            get;
            private set;
        }

        public DelegateCommand<string> CustomDownloadTab
        {
            get;
            private set;
        }

        public DelegateCommand<string> CancelTab
        {
            get;
            private set;
        }

        public DelegateCommand CollectionCommand
        {
            get;
            set;
        }

        public DelegateCommand SettingsCommand
        {
            get;
            set;
        }

        public DelegateCommand HomeCommand
        {
            get;
            set;
        }

      

        #endregion
    }
}
