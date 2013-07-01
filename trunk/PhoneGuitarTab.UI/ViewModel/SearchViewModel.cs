using GalaSoft.MvvmLight.Command;
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Core.Navigation;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.Search.UltimateGuitar;
using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.Infrastructure;
using PhoneGuitarTab.UI.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class SearchViewModel : Core.ViewModel
    {
        #region Fields

        private SearchTabResult groupSearch;

        private PageMapping pageMapping;
        private CollectionViewModel collectionViewModel;

        private SearchTabResultSummary _searchGroupTabsSummary;
        private Visibility _headerPagingVisibility;
        private IEnumerable<string> _pages;
        private TabsByName _searchGroupTabs;
        private bool _isSearching;
        private string currentSearchText;
        private bool isNothingFound = false;
        private bool isHintVisible = true;
        private SearchType searchMethod = SearchType.ByBand;
        private List<SearchType> searchOptions;

        #endregion Fields


        #region Constructors

        public SearchViewModel()
        {
            CreateCommands();

            CurrentPageIndex = 1;
            CurrentSearchText = String.Empty;

            HeaderPagingVisibility = Visibility.Collapsed;
        }

        #endregion Constructors


        #region Properties

        public SearchTabResultSummary SearchGroupTabsSummary
        {
            get { return _searchGroupTabsSummary; }
            set
            {
                _searchGroupTabsSummary = value;
                RaisePropertyChanged("SearchGroupTabsSummary");
            }
        }

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
        public IEnumerable<string> Pages
        {
            get { return _pages; }
            set
            {
                _pages = value;
                RaisePropertyChanged("Pages");
            }
        }

        public TabsByName SearchGroupTabs
        {
            get 
            { 
                return _searchGroupTabs; 
            }
            set
            {
                _searchGroupTabs = value;
                RaisePropertyChanged("SearchGroupTabs");
            }
        }

        public bool IsSearching
        {
            get 
            { 
                return _isSearching; 
            }
            set
            {
                _isSearching = value;
                RaisePropertyChanged("IsSearching");
            }
        }

        //helpers properties
        public string CurrentSearchText 
        {
            get
            {
                return currentSearchText;
            }
            set
            {
                currentSearchText = value;
                if (currentSearchText == string.Empty)
                    IsNothingFound = false;
            }
        }

        public int CurrentPageIndex { get; set; }

        public bool IsNothingFound
        {
            get
            {
                return isNothingFound;
            }
            set
            {
                isNothingFound = value;
                RaisePropertyChanged("IsNothingFound");
            }
        }

        public bool IsHintVisible
        {
            get 
            { 
                return isHintVisible; 
            }
            set 
            { 
                isHintVisible = value;
                RaisePropertyChanged("IsHintVisible");
            }
        }

        public SearchType SearchMethod
        {
            get
            {
                return searchMethod;
            }
            set
            {
                searchMethod = value;
                RaisePropertyChanged("SearchMethod");
            }
        }

        public List<SearchType> SearchOptions
        {
            get
            {
                if (searchOptions == null)
                    searchOptions = new List<SearchType>() { SearchType.ByBand, SearchType.BySong };
                return searchOptions;
            }
        }

        #endregion Properties


        #region Private properties
      
        private bool FilterTab(SearchTabResultEntry entry)
        {
            return true;
        }

        private PageMapping PageMapping
        {
            get
            {
                if (pageMapping == null)
                    pageMapping = Container.Resolve<PageMapping>();
                return pageMapping;
            }
        }

        private CollectionViewModel CollectionViewModel
        {
            get
            {
                if (collectionViewModel == null)
                    collectionViewModel = PageMapping.GetViewModel(PageType.Get(ViewType.Collection)) as CollectionViewModel;
                return collectionViewModel;
            }
        }

        #endregion Private properties


        #region Override members

        protected override void ReadNavigationParameters()
        {
            if (base.NavigationParameters.ContainsKey("SearchTerm"))
            {
                object searchTerm;
                NavigationParameters.TryGetValue("SearchTerm", out searchTerm);
                if (searchTerm != null)
                    DoLaunchSearchForBand(searchTerm.ToString());
                else
                    DoLaunchSearch(String.Empty);
            }
        }

        #endregion Override members


        #region Commands

        public RelayCommand<string> LaunchSearch
        {
            get;
            private set;
        }

        public RelayCommand<string> LaunchSearchForBand
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

        #endregion Commands


        #region Command handlers

        private void DoLaunchSearch(string arg)
        {
            SearchGroupTabs = null;
            HeaderPagingVisibility = Visibility.Collapsed;

            //string[] pattern = arg.Split(',');
            //string song = pattern.Length > 1 ? pattern[1] : "";
            //groupSearch = new SearchTabResult(pattern[0], song);

            string bandName = string.Empty;
            string songName = string.Empty;
            if (SearchMethod == SearchType.ByBand)
                bandName = arg;
            else
                songName = arg;

            groupSearch = new SearchTabResult(bandName, songName);

            IsHintVisible = false;
            IsNothingFound = false;
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

                    if (groupTabs.Count == 0)
                        IsNothingFound = true;
                    
                    Deployment.Current.Dispatcher.BeginInvoke(
                        () =>
                        {
                            SearchGroupTabsSummary = groupSearch.Summary;
                            Pages = Enumerable.Range(1, groupSearch.Summary.PageCount).Select(p => p.ToString());
                            HeaderPagingVisibility =
                                groupSearch.Summary.PageCount > 1
                                    ? Visibility.Visible
                                    : Visibility.Collapsed;
                            SearchGroupTabs = new TabsByName(groupTabs);
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

        private void DoLaunchSearchForBand(string arg)
        {
            SearchMethod = SearchType.ByBand;
            CurrentSearchText = "\"" + arg + "\"";
            HeaderPagingVisibility = Visibility.Collapsed;

            SearchGroupTabs = null;

            groupSearch = new SearchTabResult(arg, string.Empty);

            IsHintVisible = false;
            IsNothingFound = false;
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

                    if (groupTabs.Count == 0)
                        IsNothingFound = true;

                    Deployment.Current.Dispatcher.BeginInvoke(
                        () =>
                        {
                            SearchGroupTabsSummary = groupSearch.Summary;
                            Pages = Enumerable.Range(1, groupSearch.Summary.PageCount).Select(p => p.ToString());
                            HeaderPagingVisibility =
                                groupSearch.Summary.PageCount > 1
                                    ? Visibility.Visible
                                    : Visibility.Collapsed;
                            SearchGroupTabs = new TabsByName(groupTabs);
                        });
                }
                IsSearching = false;
            };

            IsSearching = true;
            groupSearch.Run(CurrentPageIndex);
        }

        private void DoSelectPage(string index)
        {
            int pageNumber;
            if (Int32.TryParse(index, out pageNumber))
            {
                CurrentPageIndex = pageNumber;
                DoLaunchSearch(CurrentSearchText);
            }
        }

        private void DoDownloadTab(string arg)
        {
            if (_isSearching)
            {
                MessageBox.Show("Sorry, you cannot download the tab right now.");
                return;
            }
            var tab =
                 SearchGroupTabs.Tabs.Where(t => t.SearchId == arg).FirstOrDefault();

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
                Tab downloadedTab = new Tab()
                {
                    Name = tab.Name,
                    Group = TabDataContextHelper.GetOrCreateGroupByName(tab.Group),
                    TabType = TabDataContextHelper.GetTabTypeByName(tab.Type),
                    Rating = tab.Rating,
                    Path = filePath
                };

                TabDataContextHelper.InsertTab(downloadedTab);

                NotifyCollection(downloadedTab);

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
                MessageBox.Show(String.Format("Error: {0}", ex));
                IsSearching = false;
            }
        }

        private bool CanDownloadTab(string arg)
        {
            //TODO test arg
            return !_isSearching;
        }

        #endregion Command handlers


        #region Helper methods

        private void CreateCommands()
        {
            CollectionCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(ViewType.Collection)));
            SettingsCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(ViewType.Settings)));
            HomeCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(ViewType.Startup)));
            LaunchSearch = new RelayCommand<string>(DoLaunchSearch, CanLaunchSearch);
            LaunchSearchForBand = new RelayCommand<string>(DoLaunchSearchForBand);
            SelectPage = new RelayCommand<string>(DoSelectPage);
            DownloadTab = new RelayCommand<string>(DoDownloadTab, CanDownloadTab);
        }

        private void NotifyCollection(Tab downloadedTab)
        {
            // TODO: use MVVM's Messager for loosely coupled design sake
            CollectionViewModel.AddDownloadedTab(downloadedTab);
        }

        #endregion Helper methods
    }
}
