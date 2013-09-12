using PhoneGuitarTab.Data;
using PhoneGuitarTab.Search;
using PhoneGuitarTab.Search.UltimateGuitar;
using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;

namespace PhoneGuitarTab.UI.ViewModel
{
    using PhoneGuitarTab.Core.Dependencies;
    using PhoneGuitarTab.Core.Views.Commands;

    public class SearchViewModel : DataContextViewModel
    {
        #region Constants

        private const int ItemsNumberForFooterVisibilityThreshold = 10;

        #endregion Constants


        #region Fields

        private UltimateGuitarTabSearcher groupSearch;

        private SearchTabResultSummary _searchGroupTabsSummary;
        private Visibility _headerPagingVisibility;
        private IEnumerable<string> _pages;
        private TabsByName _searchGroupTabs;
        private bool _isSearching;
        private string currentSearchText;
        private bool isNothingFound = false;
        private bool isHintVisible = true;
        private SearchType searchMethod = SearchType.ByBand;
        private TabulatureType searchTabType = TabulatureType.All;
        private List<SearchType> searchMethodOptions;
        private List<TabulatureType> searchTabTypeOptions;
        private TabEntity firstTabInList;

        #endregion Fields


        #region Constructors

        [Dependency]
        public SearchViewModel(IDataContextService database, MessageHub hub)
            : base(database, hub)
        {
            CreateCommands();

            CurrentPageIndex = 1;
            CurrentSearchText = String.Empty;

            HeaderPagingVisibility = Visibility.Collapsed;

            _searchGroupTabs = new TabsByName(database, true);
        }

        #endregion Constructors


        #region Properties

        [Dependency]
        private TabFileStorage TabFileStorage { get; set; }

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

        public TabulatureType SearchTabType
        {
            get 
            { 
                return searchTabType; 
            }
            set 
            {
                if (searchTabType != value)
                {
                    searchTabType = value;
                    HeaderPagingVisibility = Visibility.Collapsed;
                    if (SearchMethod == SearchType.ByBand)
                        RunSearch(CurrentSearchText, string.Empty);
                    else
                        RunSearch(string.Empty, CurrentSearchText);
                    RaisePropertyChanged("SearchTabType");
                }
            }
        }

        public List<SearchType> SearchMethodOptions
        {
            get
            {
                if (searchMethodOptions == null)
                    searchMethodOptions = new List<SearchType>() { SearchType.ByBand, SearchType.BySong };
                return searchMethodOptions;
            }
        }

        public List<TabulatureType> SearchTabTypeOptions
        {
            get
            {
                if (searchTabTypeOptions == null)
                    searchTabTypeOptions = new List<TabulatureType>() { TabulatureType.All, TabulatureType.GuitarPro, TabulatureType.Guitar, TabulatureType.Bass, TabulatureType.Chords, /*TabulatureType.Drum*/ };
                return searchTabTypeOptions;
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

        #endregion Private properties


        #region Override members

        protected override void ReadNavigationParameters()
        {
            if (NavigationParameters.ContainsKey("SearchTerm"))
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

        public ExecuteCommand<string> LaunchSearch
        {
            get;
            private set;
        }

        public ExecuteCommand<string> LaunchSearchForBand
        {
            get;
            private set;
        }

        public ExecuteCommand<string> SelectPage
        {
            get;
            private set;
        }

        public ExecuteCommand<string> DownloadTab
        {
            get;
            private set;
        }

        public ExecuteCommand CollectionCommand
        {
            get;
            set;
        }

        public ExecuteCommand SettingsCommand
        {
            get;
            set;
        }

        public ExecuteCommand HomeCommand
        {
            get;
            set;
        }

        public ExecuteCommand<TabEntity> ToggleActionArea
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
            HeaderPagingVisibility = Visibility.Collapsed;

            RunSearch(bandName, songName);
        }
        
        private void DoLaunchSearchForBand(string arg)
        {
            SearchMethod = SearchType.ByBand;
            SearchTabType = TabulatureType.All;
            CurrentPageIndex = 1;
            HeaderPagingVisibility = Visibility.Collapsed;
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
                if (SearchMethod == SearchType.ByBand)
                    RunSearch(CurrentSearchText, string.Empty);
                else
                    RunSearch(string.Empty, CurrentSearchText);
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

            //TODO create converter
            SearchTabResultEntry entry = new SearchTabResultEntry()
            {
                Id = tab.SearchId,
                Url = tab.SearchUrl,
                Type = tab.Type
            };

            var filePath = TabFileStorage.CreateTabFilePath();

            //TODO examine IO errors
            UltimateGuitarFileDownloader downloader = new UltimateGuitarFileDownloader(entry, filePath);
            downloader.DownloadComplete += delegate(object sender, DownloadCompletedEventArgs args)
            {
                if (args.HasErrors)
                    Deployment.Current.Dispatcher.BeginInvoke(
                    () =>
                    {
                        IsSearching = false;
                    });
                else
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

        private void DoToggleActionArea(TabEntity tab)
        {
            tab.ActionAreaVisible = !tab.ActionAreaVisible;
        }

        private void DoHome()
        {
            //MessengerInstance.Send<RefreshTabsMessage>(new RefreshTabsMessage());
            NavigationService.NavigateTo(Strings.Startup);
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
                    ImageUrl = Database.GetTabTypeByName(entry.Type).ImageUrl,
                    Votes = entry.Votes,
                    Version = entry.Version
                });

                if (groupTabs.Count() == 0)
                {
                    IsNothingFound = true;
                    IsSearching = false;
                    HeaderPagingVisibility = Visibility.Collapsed;
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
                        SearchGroupTabs = new TabsByName(new ObservableCollection<TabEntity>(groupTabs), Database);
                        FirstTabInList = SearchGroupTabs.GetFirstTabInFirstNonEmptyGroup();
                        SelectedPage = Pages.ElementAt(CurrentPageIndex - 1);
                        RaisePropertyChanged("SelectedPage");
                        IsSearching = false;
                    });
            }
            else
            {
                /*var toast = new ToastPrompt
                {
                    Title = "Sorry,",
                    Message = "can't reach the server right now."
                };
                toast.Show();*/
                IsSearching = false;
            }
        }

        private void DownloadTabComplete(TabEntity tab, string filePath)
        {
            Deployment.Current.Dispatcher.BeginInvoke(
                () =>
                {
                    Tab downloadedTab = new Tab()
                    {
                        Name = tab.Name,
                        Group = Database.GetOrCreateGroupByName(tab.Group),
                        TabType = Database.GetTabTypeByName(tab.Type),
                        Rating = tab.Rating,
                        Path = filePath
                    };

                    Database.InsertTab(downloadedTab);
                    Hub.RaiseTabsDownloaded();

                    tab.IsDownloaded = true;
                    IsSearching = false;

                   /* var toast = new ToastPrompt
                    {
                        Title = "\"" + tab.Name + "\" by " + tab.Group,
                        Message = " was downloaded",
                        TextOrientation = System.Windows.Controls.Orientation.Vertical
                    };
                    toast.Show();*/
                    MessageBox.Show(" was downloaded", "\"" + tab.Name + "\" by " + tab.Group, MessageBoxButton.OK);
                    //DownloadTab.RaiseCanExecuteChanged(); 
                });
        }

        #endregion Event handlers


        #region Helper methods

        private void CreateCommands()
        {
            HomeCommand = new ExecuteCommand(DoHome);
            LaunchSearch = new ExecuteCommand<string>(DoLaunchSearch);
            LaunchSearchForBand = new ExecuteCommand<string>(DoLaunchSearchForBand);
            SelectPage = new ExecuteCommand<string>(DoSelectPage);
            DownloadTab = new ExecuteCommand<string>(DoDownloadTab);
            ToggleActionArea = new ExecuteCommand<TabEntity>(DoToggleActionArea);
        }

        private void RunSearch(string bandName, string songName)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("No internet connection available.", "Can not perform operation", MessageBoxButton.OK);
                return;
            }

            SearchGroupTabs = null;
            groupSearch = new UltimateGuitarTabSearcher(bandName, songName);

            IsHintVisible = false;
            IsNothingFound = false;
            groupSearch.SearchComplete += (s, e) =>
            {
                SearchCompletedHandler(e);
            };

            IsSearching = true;
            groupSearch.Run(CurrentPageIndex, SearchTabType);
        }

        #endregion Helper methods
    }
}
