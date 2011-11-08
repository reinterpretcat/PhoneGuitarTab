using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.Search;
using PhoneGuitarTab.Search.UltimateGuitar;
using PhoneGuitarTab.UI.Controls;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class SearchViewModel : Core.ViewModel
    {
        private SearchTabResult groupSearch;
        

        public SearchViewModel()
        {

            CollectionCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(PageType.EnumType.Collection)));
            SettingsCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(PageType.EnumType.Settings)));
            HomeCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(PageType.EnumType.Startup)));
     
            LaunchSearch = new RelayCommand<string>(DoLaunchSearch, CanLaunchSearch);
            SelectPage = new RelayCommand<string>(DoSelectPage);

            CurrentPageIndex = 1;
            CurrentSearchText = String.Empty;

            DownloadTab = new RelayCommand<string>(DoDownloadTab, CanDownloadTab);
           // CustomDownloadTab = new RelayCommand<string>(DoCustomDownloadTab, CanCustomDownloadTab);
            

            /*
            SelectedCustomTabType = CustomTabTypes[0];
            //force bindin in order header rendering
            SearchGroupTabs = new TabByName(new List<Tab>());
            SearchGroupTabsSummary = new SearchTabResultSummary();
            
            CustomGroupName = "";
            CustomTabName = "";
            //Pages = Enumerable.Range(1, 1).Select(p=> new Tuple<ICommand,string>(SelectPage,p.ToString())).ToList();
            */
            HeaderPagingVisibility = Visibility.Collapsed;
        }

        private bool FilterTab(SearchTabResultEntry entry)
        {
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
                        Select(entry => new TabEntity()
                                            {
                                                            SearchId = entry.Id, 
                                                            SearchUrl = entry.Url,
                                                            Name = entry.Name,
                                                            Group = entry.Artist, 
                                                            Rating = entry.Rating,
                                                            Type = entry.Type,
                                                            ImageUrl = TabDataContextHelper.GetTabTypeByName(entry.Type).ImageUrl
                                                        }).ToList();
                    Deployment.Current.Dispatcher.BeginInvoke(
                        () =>
                            {
                                SearchGroupTabsSummary = groupSearch.Summary;
                                Pages = Enumerable.Range(1, groupSearch.Summary.PageCount).Select(p=>p.ToString());
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

        private void DoSelectPage(string index)
        {
            int pageNumber;
            if(Int32.TryParse(index, out pageNumber))
            {
                CurrentPageIndex = pageNumber;
                DoLaunchSearch(CurrentSearchText);
            }
        }

        /* private bool CanCustomDownloadTab(string arg)
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

           
             var filePath = TabStorageHelper.CreateTabFilePath();
             var fileStream = TabStorageHelper.CreateTabFile(filePath);

             FileDownloader downloader = new FileDownloader(fileStream);
             downloader.DownloadComplete += delegate
                                                {
                                                    Tab tab = new Tab()
                                                                  {
                                                                      TabType = TabDataContextHelper.GetTabTypeByName(customType),
                                                                      Name = customTab
                                                                  };

                 tab.Path = filePath;
                 //Note get or create new instance from/in database
                 tab.Group = TabDataContextHelper.GetOrCreateGroupByName(customGroup);

                 TabDataContextHelper.InsertTab(tab);

                 Deployment.Current.Dispatcher.BeginInvoke(
                     () =>
                     {
                         IsSearching = false;
                         MessageBox.Show("Your tab was downloaded");
                     });
             };
             IsSearching = true;

             downloader.Download(url);

         }*/

        private void DoDownloadTab(string arg)
        {
            if(_isSearching)
            {
                MessageBox.Show("Sorry, you cannot download the tab right now.");
                return;
            }
           var tab =
                SearchGroupTabs.AllTabs.Where(t => t.SearchId == arg).FirstOrDefault();

            //TODO create converter
            SearchTabResultEntry entry = new SearchTabResultEntry()
                                             {
                                                 Id = tab.SearchId,
                                                 Url = tab.SearchUrl,
                                                 Type = tab.Description
                                             };

            var filePath = TabStorageHelper.CreateTabFilePath();

            //TODO examine IO errors
            SearchTabDownloader downloader = new SearchTabDownloader(entry, filePath);
            downloader.DownloadComplete += delegate
            {
                Tab t = new Tab()
                {
                    Name = tab.Name,
                    Group = TabDataContextHelper.GetOrCreateGroupByName(tab.Group),
                    TabType =TabDataContextHelper.GetTabTypeByName(tab.Type),
                    Rating = tab.Rating,
                    Path = filePath
                };
                
                TabDataContextHelper.InsertTab(t);

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
                /*if(!UgSession.Instance.IsAuthenticated)
                {
                    UgSession.Instance.BeginLogin("", "", ar => downloader.Download(), null);
                }
                else
                {*/
                    downloader.Download();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error: {0}",ex));
                IsSearching = false;
            }
        }

        private bool CanDownloadTab(string arg)
        {
            //TODO test arg
            return !_isSearching;
        }

        private string _customGroupName;
        public string CustomGroupName
        {
            get { return _customGroupName; }
            set
            {
                _customGroupName = value;
                RaisePropertyChanged("CustomGroupName");
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
                RaisePropertyChanged("CustomTabName");
                CustomDownloadTab.RaiseCanExecuteChanged();
            }
        }

        private List<Core.Tuple<string, string>> _customTabTypes;
        /*public List<Core.Tuple<string,string>> CustomTabTypes
        {
            get
            {
                //create type-image mapping
                if(_customTabTypes == null)
                {
                    _customTabTypes = new List<Core.Tuple<string, string>>();
                    foreach (string key in TabDataContextHelper.TabImageTypeMapping.Keys)
                        _customTabTypes.Add(new Core.Tuple<string, string>(key, TabDataContextHelper.TabImageTypeMapping[key]));
                }

                return _customTabTypes;
            }
        }*/

        private Core.Tuple<string, string> _selectedCustomTabType;
        public Core.Tuple<string, string> SelectedCustomTabType
        {
            get { return _selectedCustomTabType; }
            set
            {
                _selectedCustomTabType = value;
                RaisePropertyChanged("CustomTabName");
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
                RaisePropertyChanged("SearchGroupTabsSummary");
            }
        }

        private Visibility _headerPagingVisibility;
        public Visibility HeaderPagingVisibility
        {
            get { return _headerPagingVisibility; }
            set
            {
                _headerPagingVisibility = value;
                RaisePropertyChanged("HeaderPagingVisibility");
            }
        }

        //NOTE I cannot bind Command in ItemsControl with custom ItemPanelTemplate
        //So this type is used instead
        private IEnumerable<string> _pages;
        public IEnumerable<string> Pages
        {
            get { return _pages; }
            set
            {
                _pages = value;
                RaisePropertyChanged("Pages");
            }
        }

        private TabByName _searchGroupTabs;
        public TabByName SearchGroupTabs
        {
            set
            {
                _searchGroupTabs = value;
                RaisePropertyChanged("SearchGroupTabs");
            }
            get { return _searchGroupTabs; }

        }

        private bool _isSearching;
        public bool IsSearching
        {
            set
            {
                _isSearching = value;
                RaisePropertyChanged("IsSearching");
            }
            get { return _isSearching; }
        }

        //helpers properties
        public string CurrentSearchText { get; set; }
        public int CurrentPageIndex { get; set; }

        #region Properties

        public RelayCommand<string> LaunchSearch
        {
            get;
            private set;
        }

        public RelayCommand<string> SelectPage
        {
            get;
            private set;
        }

        public RelayCommand<string> DownloadTab
        {
            get;
            private set;
        }

        public RelayCommand<string> CustomDownloadTab
        {
            get;
            private set;
        }

      
        public RelayCommand CollectionCommand
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
    }
}
