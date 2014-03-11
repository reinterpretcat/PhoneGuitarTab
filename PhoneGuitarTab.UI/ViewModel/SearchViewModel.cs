using System.Windows.Input;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.Search;
using PhoneGuitarTab.Search.UltimateGuitar;
using PhoneGuitarTab.Search.Lastfm;
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
        private HorizontalAlignment _pagesListAlignment;
        private Thickness _pagingListPadding;
        private IEnumerable<string> _pages;
        private TabsByName _searchGroupTabs;
        private bool _isSearching;
        private string currentSearchText;
        private string currentTypedText;
        private string searchInfoTextBlock;
        private string waterMarkText = " type here..";
        private bool isDownloading;
        private bool isSearchButtonEnabled;
        private bool isNothingFound = false;
        private bool isHintVisible = true;
        private SearchType searchMethod = SearchType.ByBand;
        private TabulatureType searchTabType = TabulatureType.All;
        private List<SearchType> searchMethodOptions;
        private List<TabulatureType> searchTabTypeOptions;
        private TabEntity firstTabInList;
        private bool downloadButtonClicked = false;
        private Tab currentTab;
        private TabEntity currentTabEntity;
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
        private IDialogController Dialog { get; set; }

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

        public HorizontalAlignment PagesListAlignment
        {
            get { return _pagesListAlignment; }
            set 
            {
                _pagesListAlignment = value;
                RaisePropertyChanged("PagesListAlignment");
            }
        }

        public Thickness PagingListPadding
        {
            get { return _pagingListPadding; }
            set
            {
                _pagingListPadding = value;
                RaisePropertyChanged("PagingListPadding");
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
               
            }
        }

        public bool IsDownloading 
        {
            get
            {
                return isDownloading;
            }
            set
            {
                isDownloading = value;
                RaisePropertyChanged("IsDownloading");
                RaisePropertyChanged("CanDownload");
            }
        }
        public bool CanDownload
        {
            get { return !IsDownloading; }
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

        public string CurrentTypedText
        {
            get
            {
                return currentTypedText;
            }
            set
            {
                currentTypedText = value;
                RaisePropertyChanged("CurrentTypedText");
            }
        }

        public string SearchInfoTextBlock
        {
            get
            {
                return searchInfoTextBlock;
            }
            set
            {
                searchInfoTextBlock = value;
                RaisePropertyChanged("SearchInfoTextBlock");
            }
        }

        public string WaterMarkText 
        {
            get
            {
                return waterMarkText;
            }
            set
            {
                waterMarkText = value;
                RaisePropertyChanged("WaterMarkText");
            }
        }

        public int CurrentPageIndex { get; set; }

        public bool IsSearchButtonEnabled 
        {
            get
            {
                return isSearchButtonEnabled;
            }
            set
            {
                isSearchButtonEnabled = value;
                RaisePropertyChanged("IsSearchButtonEnabled");
            }
        
        }

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
                if (isNothingFound)
                {
                    this.SearchMethod = SearchType.BandSong;
                    RaisePropertyChanged("SearchMethod"); 
                }
               
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
                CurrentPageIndex = 1;
                if (searchTabType != value)
                {
                    searchTabType = value;
                    this.DoLaunchSearch(this.CurrentSearchText);
                    RaisePropertyChanged("SearchTabType");
                }
            }
        }

        public List<SearchType> SearchMethodOptions
        {
            get
            {
                if (searchMethodOptions == null)
                    searchMethodOptions = new List<SearchType>() { SearchType.ByBand, SearchType.BySong, SearchType.BandSong };
                return searchMethodOptions;
            }
        }

        public List<TabulatureType> SearchTabTypeOptions
        {
            get
            {
                if (searchTabTypeOptions == null)
                    searchTabTypeOptions = new List<TabulatureType>() { TabulatureType.All, TabulatureType.GuitarPro, TabulatureType.Guitar, TabulatureType.Bass, TabulatureType.Chords, TabulatureType.Drum };
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


        public Tab CurrentTab
        {
            get
            {
                return currentTab;
            }
            set
            {
                currentTab = value;
                RaisePropertyChanged("CurrentTab");
            }
        }

        public TabEntity CurrentTabEntity
        {
            get
            {
                return currentTabEntity;
            }
            set
            {
                currentTabEntity = value;
                RaisePropertyChanged("CurrentTabEntity");
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

        public ExecuteCommand<string> NavigatePage 
        {
            get;
            private set;
        }
        /*public ExecuteCommand CollectionCommand
        {
            get;
            set;
        }*/

       /* public ExecuteCommand SettingsCommand
        {
            get;
            set;
        }*/

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
            switch (SearchMethod)
            { 
                case SearchType.ByBand:
                     bandName = CurrentSearchText = arg;
                     break;
                case SearchType.BySong:
                     songName = CurrentSearchText = arg;
                     break;
                case SearchType.BandSong:
                     if (arg.Contains(","))
                     {
                         CurrentSearchText = arg;
                         string[] words = arg.Split(',');
                         bandName = words[0].Trim();
                         songName = words[1].Trim();
                     }
                     else if (arg.Contains(" "))
                     {
                         CurrentSearchText = arg;
                         string[] words = arg.Split(' ');
                         bandName = words[0].Trim();
                         for (int i = 1; i < words.Length; i++)
                         {
                             songName += words[i];
                         }
                     }
                     else
                     { bandName = CurrentSearchText = arg; }
                    
                     break;

            }
            
            CurrentPageIndex = 1;
            HeaderPagingVisibility = Visibility.Collapsed;
            SearchInfoTextBlock = "Searching for " + CurrentTypedText + "..";
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
                this.SearchInfoTextBlock = "Page " + index + " for " + this.CurrentTypedText + ".."; 
                CurrentPageIndex = pageNumber;

                string bandName = string.Empty,
                    songName = string.Empty;

                switch (SearchMethod)
                {  
                    case SearchType.ByBand:
                        bandName = CurrentSearchText;
                        break;
                    case SearchType.BySong:
                        songName = CurrentSearchText;
                        break;
                    case SearchType.BandSong:
                            if (CurrentSearchText.Contains(","))
                            { 
                                string[] words = CurrentSearchText.Split(',');
                                bandName = words[0].Trim();
                                songName = words[1].Trim();
                            }
                            else if (CurrentSearchText.Contains(" "))
                            {

                                string[] words = CurrentSearchText.Split(' ');
                                bandName = words[0].Trim();
                                for (int i = 1; i < words.Length; i++)
                                {
                                    songName += words[i];
                                }
                            }
                            else { bandName = CurrentSearchText; }
                        break;
                }
                RunSearch(bandName, songName);

            }
        }

        private void DoNavigatePage(string direction)
        {
            if (direction == null)
                return;
            
            if (direction == "next")
            {
                if (!(this.Pages.Count() == CurrentPageIndex))
                {
                
                    this.DoSelectPage((CurrentPageIndex + 1).ToString());
                }
            }
            else if (direction == "previous")
            {
                if (!(CurrentPageIndex == 1))
                {
                   
                    this.DoSelectPage((CurrentPageIndex - 1).ToString());
                }
            }
        }

        private void DoDownloadTab(string arg)
        {
            downloadButtonClicked = true;

            if (IsDownloading)
            {
                Dialog.Show("Sorry, you cannot download the tab right now.");
                return;
            }
            TabEntity tab = SearchGroupTabs.Tabs.FirstOrDefault(t => t.SearchId == arg);

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
                        IsDownloading = false;
                    });
                else
                    DownloadTabComplete(tab, filePath);
            };
            SearchInfoTextBlock = "Downloading " + tab.Name + "..";
            IsDownloading = true;
            try
            {
                downloader.Download();
            }
            catch (Exception ex)
            {
                Dialog.Show(String.Format("Error: {0}", ex));
                IsDownloading = false;
            }
        }

        private void DoToggleActionArea(TabEntity tab)
        {
            if (!downloadButtonClicked)
            {
                tab.ActionAreaVisible = !tab.ActionAreaVisible;
            }
            downloadButtonClicked = false;
        }

        private void DoHome()
        {
            Hub.RaiseTabsRefreshed();
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

                IsNothingFound = !groupTabs.Any();
                Deployment.Current.Dispatcher.BeginInvoke(
                    () =>
                    {
                      
                        Pages = Enumerable.Range(1, groupSearch.Summary.PageCount).Select(p => p.ToString());
                        
                        SearchGroupTabs = new TabsByName(new ObservableCollection<TabEntity>(groupTabs), Database);
                        FirstTabInList = SearchGroupTabs.GetFirstTabInFirstNonEmptyGroup();
                        if(Pages.Any())
                            SelectedPage = Pages.ElementAt(CurrentPageIndex - 1);
                        RaisePropertyChanged("SelectedPage");
                        this.AssignHeaderPagingUI(groupSearch.Summary.PageCount);
                        IsSearching = false;
                    });
            }
            else
            {
                IsSearching = false;
                Dialog.Show("Sorry,", "can't reach the server right now.");
               
            }
        }

        private void DownloadTabComplete(TabEntity tab, string filePath)
        {
            this.CurrentTabEntity = tab;
            Deployment.Current.Dispatcher.BeginInvoke(
              () =>
              {
                  this.CurrentTab = new Tab()
                  {
                      Name = tab.Name,
                      Group = Database.GetOrCreateGroupByName(tab.Group),
                      TabType = Database.GetTabTypeByName(tab.Type),
                      Rating = tab.Rating,
                      Path = filePath,
                  };
                  Database.InsertTab(this.CurrentTab);
                  Hub.RaiseTabsDownloaded();

                  this.CurrentTabEntity.IsDownloaded = true;
                  IsDownloading = false;

                  Dialog.Show(" was downloaded", "\"" + this.CurrentTabEntity.Name + "\" by " + this.CurrentTabEntity.Group, new DialogActionContainer()
                  {
                      OnTapAction = (o, e) => NavigationService.NavigateToTab(this.CurrentTab)
                  });
                

                  DownloadTab.RaiseCanExecuteChanged();
                 
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
            NavigatePage = new ExecuteCommand<string>(DoNavigatePage);
        }

        private void RunSearch(string bandName, string songName)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                Dialog.Show("No internet connection available.", "Can not perform operation");
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

        private void AssignHeaderPagingUI(int pageCount)
        {
          

            PagesListAlignment = pageCount < 7
                ? HorizontalAlignment.Center
                : HorizontalAlignment.Center;
        
            PagingListPadding = pageCount < 8
                ? new Thickness(14)
                : new Thickness(6.5);

            HeaderPagingVisibility = pageCount > 1
                              ? Visibility.Visible
                              : Visibility.Collapsed;
        }
      
        #endregion Helper methods
    }
}
