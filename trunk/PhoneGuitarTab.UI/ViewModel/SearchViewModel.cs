using Coding4Fun.Toolkit.Controls;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Controls;
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
using System.Windows.Controls;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class SearchViewModel : Core.ViewModel
    {
        #region Constants

        private const int ItemsNumberForFooterVisibilityThreshold = 10;

        #endregion Constants


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
        private TabEntity firstTabInList;
        private TabEntity selectedTab;

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

        public string SelectedPage { get; set; }

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
                RaisePropertyChanged("IsFooterNeeded");
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
                RaisePropertyChanged("CanDownload");
            }
        }

        public bool CanDownload
        {
            get { return !IsSearching; }
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
                RaisePropertyChanged("CurrentSearchBand");
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

        public bool IsFooterNeeded
        {
            get
            {
                return SearchGroupTabs != null && (SearchGroupTabs.Tabs.Count > ItemsNumberForFooterVisibilityThreshold);
            }
        }

        /// <summary>
        /// used for "jump to top" feature
        /// </summary>
        public TabEntity FirstTabInList
        {
            get 
            { 
                return firstTabInList; 
            }
            set 
            { 
                firstTabInList = value;
                RaisePropertyChanged("FirstTabInList");
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

        public RelayCommand<SelectionChangedEventArgs> SelectTab
        {
            get;
            private set;
        }

        public RelayCommand<TabEntity> HideActionArea
        {
            get;
            set;
        }

        #endregion Commands


        #region Command handlers

        private void DoLaunchSearch(string arg)
        {
            string  bandName = string.Empty, 
                    songName = string.Empty;
            if (SearchMethod == SearchType.ByBand)
                bandName = CurrentSearchText = arg;
            else
                songName = CurrentSearchText = arg;

            CurrentPageIndex = 1;

            RunSearch(bandName, songName);
        }
        
        private void DoLaunchSearchForBand(string arg)
        {
            SearchMethod = SearchType.ByBand;
            CurrentPageIndex = 1;
            CurrentSearchText = arg;

            RunSearch(CurrentSearchText, string.Empty);
        }

        private void DoSelectPage(string index)
        {
            if (index == null)
                return;

            int pageNumber;
            if (Int32.TryParse(index, out pageNumber))
            {
                CurrentPageIndex = pageNumber;
                RunSearch(CurrentSearchText, string.Empty);
            }
        }

        private void DoDownloadTab(string arg)
        {
            if (IsSearching)
            {
                MessageBox.Show("Sorry, you cannot download the tab right now.");
                return;
            }
            TabEntity tab = SearchGroupTabs.Tabs.Where(t => t.SearchId == arg).FirstOrDefault();
            selectedTab.ActionAreaVisible = false;

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
                DownloadTabComplete(tab, filePath);
            };
            IsSearching = true;
            try
            {
                downloader.Download();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error: {0}", ex));
                IsSearching = false;
            }
        }

        private void DoSelectTab(SelectionChangedEventArgs args)
        {
            if (args == null || args.AddedItems.Count == 0)
            {
                if (selectedTab != null)
                    selectedTab.ActionAreaVisible = false;
                return;
            }

            var selectedItem = args.AddedItems[0] as LongListSelectorItem;
            if (selectedItem != null && selectedItem.Item != null)
            {
                if (selectedTab != null)
                    selectedTab.ActionAreaVisible = false;
                selectedTab = ((TabEntity)selectedItem.Item);
                selectedTab.ActionAreaVisible = true;
            }
        }

        private void DoHideActionArea(TabEntity tab)
        {
            if (tab == selectedTab)
                selectedTab.ActionAreaVisible = !selectedTab.ActionAreaVisible;
        }

        #endregion Command handlers


        #region Event handlers

        private void SearchCompletedHandler(System.Net.DownloadStringCompletedEventArgs e)
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
                    ImageUrl = TabDataContextHelper.GetTabTypeByName(entry.Type).ImageUrl,
                    Votes = entry.Votes,
                    Version = entry.Version
                }).ToList();

                if (groupTabs.Count == 0)
                {
                    IsNothingFound = true;
                    IsSearching = false;
                    return;
                }
                Deployment.Current.Dispatcher.BeginInvoke(
                    () =>
                    {
                        Pages = Enumerable.Range(1, groupSearch.Summary.PageCount).Select(p => p.ToString());
                        HeaderPagingVisibility =
                            groupSearch.Summary.PageCount > 1
                                ? Visibility.Visible
                                : Visibility.Collapsed;
                        SearchGroupTabs = new TabsByName(groupTabs);
                        FirstTabInList = SearchGroupTabs.GetFirstTabInFirstNonEmptyGroup();
                        SelectedPage = Pages.ElementAt(CurrentPageIndex - 1);
                        RaisePropertyChanged("SelectedPage");
                    });
            }
            else
            {
                var toast = new ToastPrompt
                {
                    Title = "Sorry,",
                    Message = "can't reach the server right now."
                };
                toast.Show();
            }
            IsSearching = false;
        }

        private void DownloadTabComplete(TabEntity tab, string filePath)
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
                    var toast = new ToastPrompt
                    {
                        Title = "\"" + tab.Name + "\" by " + tab.Group,
                        Message = " was downloaded",
                        TextOrientation = System.Windows.Controls.Orientation.Vertical
                    };
                    toast.Show();

                    tab.IsDownloaded = true;

                    IsSearching = false;
                    DownloadTab.RaiseCanExecuteChanged();
                });
        }

        #endregion Event handlers


        #region Helper methods

        private void CreateCommands()
        {
            CollectionCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(ViewType.Collection)));
            SettingsCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(ViewType.Settings)));
            HomeCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(ViewType.Startup)));
            LaunchSearch = new RelayCommand<string>(DoLaunchSearch);
            LaunchSearchForBand = new RelayCommand<string>(DoLaunchSearchForBand);
            SelectPage = new RelayCommand<string>(DoSelectPage);
            DownloadTab = new RelayCommand<string>(DoDownloadTab);
            SelectTab = new RelayCommand<SelectionChangedEventArgs>(DoSelectTab);
            HideActionArea = new RelayCommand<TabEntity>(DoHideActionArea);
        }

        private void NotifyCollection(Tab downloadedTab)
        {
            // TODO: use MVVM's Messager for loosely coupled design sake
            CollectionViewModel.AddDownloadedTab(downloadedTab);
        }

        private void RunSearch(string bandName, string songName)
        {
            HeaderPagingVisibility = Visibility.Collapsed;
            SearchGroupTabs = null;

            groupSearch = new SearchTabResult(bandName, songName);

            IsHintVisible = false;
            IsNothingFound = false;
            groupSearch.SearchComplete += (s, e) =>
            {
                SearchCompletedHandler(e);
            };

            IsSearching = true;
            groupSearch.Run(CurrentPageIndex);
        }

        #endregion Helper methods
    }
}
