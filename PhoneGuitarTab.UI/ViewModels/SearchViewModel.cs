using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Views.Commands;
using PhoneGuitarTab.Search.Arts;
using PhoneGuitarTab.Search.Extensions;
using PhoneGuitarTab.Search.Tabs;
using PhoneGuitarTab.UI.Data;
using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.Infrastructure;
using PhoneGuitarTab.UI.Resources;

namespace PhoneGuitarTab.UI.ViewModels
{
    public class SearchViewModel : DataContextViewModel
    {
      
        #region Fields

        private readonly IMediaSearcherFactory _mediaSearcherFactory; 
        private readonly ITabSearcher _tabSearcher;
        private SearchTabResultSummary _searchGroupTabsSummary;
        private Visibility _headerPagingVisibility;
        private HorizontalAlignment _pagesListAlignment;
        private Thickness _pagingListPadding;
        private IEnumerable<string> _pages;
        private TabsByName _searchGroupTabs;
        private ObservableCollection<TabEntity> _searchPopularTabs;
        private bool _isSearching;
        private string currentSearchText;
        private string currentTypedText;
        private string searchInfoTextBlock;
        private string waterMarkText = " type here..";
        private bool isDownloading;
        private bool isSearchButtonEnabled;
        private bool isNothingFound;
        private bool isHintVisible = true;
        private SearchType searchMethod = SearchType.ByBand;
        private TabulatureType searchTabType = TabulatureType.All;
        private List<SearchType> searchMethodOptions;
        private List<TabulatureType> searchTabTypeOptions;
        private TabEntity firstTabInList;
        private bool downloadButtonClicked;
        private Tab currentTab;
        private TabEntity currentTabEntity;
        private ResultsSortOrder _sortOrder;
        #endregion Fields

        #region Constructors

        [Dependency]
        public SearchViewModel(IMediaSearcherFactory mediaSearcherFactory, ITabSearcher tabSearcher, IDataContextService database, MessageHub hub)
            : base(database, hub)
        {
            CreateCommands();

            CurrentPageIndex = 1;
            CurrentSearchText = String.Empty;

            HeaderPagingVisibility = Visibility.Collapsed;

            _searchGroupTabs = new TabsByName(database, true);         
            _tabSearcher = tabSearcher;
            _tabSearcher.SearchComplete += (s, e) => SearchCompletedHandler(e);
            _mediaSearcherFactory = mediaSearcherFactory;
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
            get { return _searchGroupTabs; }
            set
            {
                _searchGroupTabs = value;
                RaisePropertyChanged("SearchGroupTabs");
               
            }
        }

        public ObservableCollection<TabEntity> SearchPopularTabs
        {
            get { return _searchPopularTabs; }
            set
            {
                _searchPopularTabs = value;
                RaisePropertyChanged("SearchPopularTabs");
            }
        }

        public bool IsSearching
        {
            get { return _isSearching; }
            set
            {
                _isSearching = value;
                RaisePropertyChanged("IsSearching");
            }
        }

        public bool IsDownloading
        {
            get { return isDownloading; }
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

        public string CurrentSearchText
        {
            get { return currentSearchText; }
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
            get { return currentTypedText; }
            set
            {
                currentTypedText = value;
                RaisePropertyChanged("CurrentTypedText");
            }
        }

        public string SearchInfoTextBlock
        {
            get { return searchInfoTextBlock; }
            set
            {
                searchInfoTextBlock = value;
                RaisePropertyChanged("SearchInfoTextBlock");
            }
        }

        public string WaterMarkText
        {
            get { return waterMarkText; }
            set
            {
                waterMarkText = value;
                RaisePropertyChanged("WaterMarkText");
            }
        }

        public int CurrentPageIndex { get; set; }

        public bool IsSearchButtonEnabled
        {
            get { return isSearchButtonEnabled; }
            set
            {
                isSearchButtonEnabled = value;
                RaisePropertyChanged("IsSearchButtonEnabled");
            }
        }

        public bool IsNothingFound
        {
            get { return isNothingFound; }
            set
            {
                isNothingFound = value;
                RaisePropertyChanged("IsNothingFound");
                if (isNothingFound)
                {
                    SearchMethod = SearchType.BandSong;
                    RaisePropertyChanged("SearchMethod");
                }
            }
        }

        public bool IsHintVisible
        {
            get { return isHintVisible; }
            set
            {
                isHintVisible = value;
                RaisePropertyChanged("IsHintVisible");
            }
        }

        public SearchType SearchMethod
        {
            get { return searchMethod; }
            set
            {
                searchMethod = value;
                RaisePropertyChanged("SearchMethod");
            }
        }

        public TabulatureType SearchTabType
        {
            get { return searchTabType; }
            set
            {
                CurrentPageIndex = 1;
                if (searchTabType != value)
                {
                    searchTabType = value;
                    DoLaunchSearch(CurrentSearchText);
                    RaisePropertyChanged("SearchTabType");
                }
            }
        }

        public List<SearchType> SearchMethodOptions
        {
            get
            {
                if (searchMethodOptions == null)
                    searchMethodOptions = new List<SearchType>
                    {
                        SearchType.ByBand,
                        SearchType.BySong,
                        SearchType.BandSong
                    };
                return searchMethodOptions;
            }
        }

        public List<TabulatureType> SearchTabTypeOptions
        {
            get
            {
                if (searchTabTypeOptions == null)
                    searchTabTypeOptions = new List<TabulatureType>
                    {
                        TabulatureType.All,
                        TabulatureType.GuitarPro,
                        TabulatureType.Guitar,
                        TabulatureType.Bass,
                        TabulatureType.Chords,
                        TabulatureType.Drum
                    };
                return searchTabTypeOptions;
            }
        }

     
        public Tab CurrentTab
        {
            get { return currentTab; }
            set
            {
                currentTab = value;
                RaisePropertyChanged("CurrentTab");
            }
        }
        public TabEntity CurrentTabEntity
        {
            get { return currentTabEntity; }
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

        public ExecuteCommand<string> LaunchSearch { get; private set; }

        public ExecuteCommand<string> LaunchSearchForBand { get; private set; }

        public ExecuteCommand<string> LaunchSearchPopularTabs { get; private set; }

        public ExecuteCommand<string> SelectPage { get; private set; }

        public ExecuteCommand<string> DownloadTab { get; private set; }

        public ExecuteCommand<int> GoToTabView { get; private set; }

        public ExecuteCommand<string> NavigatePage { get; private set; }

        public ExecuteCommand HomeCommand { get; set; }

        public ExecuteCommand<TabEntity> ToggleActionArea { get; set; }

        #endregion Commands

        #region Command handlers

        private void DoLaunchSearch(string arg)
        {
            string bandName = string.Empty,
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
                            songName += words[i] + " ";
                        }
                    }
                    else
                    {
                        bandName = CurrentSearchText = arg;
                    }

                    break;
            }

            CurrentPageIndex = 1;
            HeaderPagingVisibility = Visibility.Collapsed;
            SearchInfoTextBlock = String.Format(AppResources.Search_SearchFor, CurrentTypedText);
            RunSearch(bandName.TransLiterate(), songName.TransLiterate(), ResultsSortOrder.Alphabetical);
        }

        private void DoLaunchSearchForBand(string arg)
        {
            SearchMethod = SearchType.ByBand;
            SearchTabType = TabulatureType.All;
            CurrentPageIndex = 1;
            HeaderPagingVisibility = Visibility.Collapsed;
            CurrentSearchText = arg;
            RunSearch(CurrentSearchText.TransLiterate(), string.Empty, ResultsSortOrder.Alphabetical);
        }

         private void DoLaunchSearchPopularTabs(string arg)
         {
           bool shouldRun = (this.SearchPopularTabs == null || this.SearchPopularTabs.Count == 0 || this.SearchPopularTabs.FirstOrDefault().Group.TransLiterate() != arg.TransLiterate()) && !IsSearching;


           if (shouldRun && NetworkInterface.GetIsNetworkAvailable())
             {
                 this.SearchPopularTabs = null;
                 SearchMethod = SearchType.ByBand;
                 SearchTabType = TabulatureType.All;
                 CurrentPageIndex = 1;
                 CurrentSearchText = arg;
                 RunSearch(arg.TransLiterate(), string.Empty, ResultsSortOrder.Popularity);
             }
               
        }

        private void DoSelectPage(string index)
        {
            if (index == null)
                return;

            int pageNumber;
            if (Int32.TryParse(index, out pageNumber))
            {
                SearchInfoTextBlock = String.Format(AppResources.Search_PageFor, index, CurrentTypedText);
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
                                songName += words[i] + " ";
                            }
                        }
                        else
                        {
                            bandName = CurrentSearchText;
                        }
                        break;
                }
                RunSearch(bandName, songName, ResultsSortOrder.Alphabetical);
            }
        }

        private void DoNavigatePage(string direction)
        {
            if (direction == null)
                return;

            if (direction == "next")
            {
                if (Pages.Count() != CurrentPageIndex)
                {
                    DoSelectPage((CurrentPageIndex + 1).ToString());
                }
            }
            else if (direction == "previous")
            {
                if (CurrentPageIndex != 1)
                {
                    DoSelectPage((CurrentPageIndex - 1).ToString());
                }
            }
        }

        private void DoDownloadTab(string arg)
        {
            downloadButtonClicked = true;

            if (IsDownloading)
            {
                Dialog.Show(AppResources.Search_DownloadFailed);
                return;
            }
            TabEntity tab;
           
            if (_sortOrder == ResultsSortOrder.Alphabetical)
                tab = SearchGroupTabs.Tabs.FirstOrDefault(t => t.SearchId == arg);
            else
                tab = SearchPopularTabs.FirstOrDefault(t => t.SearchId == arg);
            //TODO create converter
            var entry = new SearchTabResultEntry
            {
                Id = tab.SearchId,
                Url = tab.SearchUrl,
                Type = tab.Type
            };

            var filePath = TabFileStorage.CreateTabFilePath();

            // TODO Do not depend on implementation!
            // TODO examine IO errors
            var downloader = new UltimateGuitarFileDownloader(entry, filePath);

            downloader.DownloadComplete += delegate(object sender, DownloadCompletedEventArgs args)
            {
                if (args.HasErrors)
                    Deployment.Current.Dispatcher.BeginInvoke(
                        () => { IsDownloading = false; });
                else
                    DownloadTabComplete(tab, filePath);
            };
            SearchInfoTextBlock = String.Format(AppResources.Search_Downloading, tab.Name);
            IsDownloading = true;
            try
            {
                downloader.Download();
            }
            catch (Exception ex)
            {
                Dialog.Show(String.Format(AppResources.Search_Error, ex));
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

        private void DoGoToTabView(int id)
        {
            Tab tab = (from Tab t in Database.Tabs
                       where t.Id == id
                       select t).Single();
            NavigationService.NavigateToTab(new Dictionary<string, object> { { "Tab", tab } });
            Hub.RaiseBackGroundImageChangeActivity(tab.Group.ExtraLargeImageUrl);
            Hub.RaiseTabBrowsed();
        }

        #endregion Command handlers

        #region Event handlers

        private void SearchCompletedHandler( System.Net.DownloadStringCompletedEventArgs e)
        {
            SearchGroupTabs = null;
            //TODO examine e.Error 
            if (e.Error == null)
            {
                            
              var groupTabs = _tabSearcher.Entries.Where(FilterTab).
                    Select(entry => new TabEntity
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

                if (_sortOrder == ResultsSortOrder.Popularity)
                {
                    var artistName = CurrentSearchText.TransLiterate();
                    var grouped = groupTabs.Where(t => t.Group.TransLiterate() == artistName).OrderByDescending(t => t.Votes);

                    SearchPopularTabs = new ObservableCollection<TabEntity>(grouped.Take(100));
                    Deployment.Current.Dispatcher.BeginInvoke(() => { IsSearching = false; });
                    //Clear the Search Area (for the Search Page)
                    this.ClearSearchArea();

                }
                else
                {
                    Pages = Enumerable.Range(1, _tabSearcher.Summary.PageCount).Select(p => p.ToString());
                    SearchGroupTabs = new TabsByName(new ObservableCollection<TabEntity>(groupTabs), Database);
                    Deployment.Current.Dispatcher.BeginInvoke(
                   () =>
                   {
                       if (Pages.Any())
                           SelectedPage = Pages.ElementAt(CurrentPageIndex - 1);
                       RaisePropertyChanged("SelectedPage");
                       AssignHeaderPagingUI(_tabSearcher.Summary.PageCount);
                       IsSearching = false;
                   });
                }

               
                             
            }
            else
            {
                IsSearching = false;
                ClearSearchArea();
                Dialog.Show(AppResources.Search_Sorry, AppResources.Search_ServerUnavailable);
            }
        }

        private void DownloadTabComplete(TabEntity tab, string filePath)
        {
            CurrentTabEntity = tab;
           // TabEntity CurrentPopularTabEntity = SearchPopularTabs.FirstOrDefault(t => t.SearchId == tab.SearchId);
            Deployment.Current.Dispatcher.BeginInvoke(
                () =>
                {
                    if (!Database.IsGroupExists(CurrentTabEntity.Group))
                    {
                        //Raise Band downloaded
                        Hub.RaiseBandCreated();
                    }
                    CurrentTab = new Tab
                    {
                        Name = tab.Name,
                        Group = Database.GetOrCreateGroupByName(tab.Group),
                        TabType = Database.GetTabTypeByName(tab.Type),
                        Rating = tab.Rating,
                        Path = filePath,
                    };
                    Database.InsertTab(CurrentTab);
                    //run album images search
                    var tabAlbumSearch = _mediaSearcherFactory.Create();
                    tabAlbumSearch.MediaSearchCompleted += tabAlbumSearch_MediaSearchCompleted;
                    tabAlbumSearch.RunMediaSearch(CurrentTab.Group.Name, CurrentTab.Name);
                    Hub.RaiseTabsDownloaded();

                        //run group images search
                        var groupImagesSearch = _mediaSearcherFactory.Create();
                        groupImagesSearch.MediaSearchCompleted += groupImagesSearch_MediaSearchCompleted;
                        groupImagesSearch.RunMediaSearch(tab.Group, string.Empty);
                  
                    CurrentTabEntity.IsDownloaded =  true;
                    CurrentTabEntity.Id =  CurrentTab.Id;
                    IsDownloading = false;

                    Dialog.Show(AppResources.Search_TabDownloadedText, 
                        String.Format(AppResources.Search_TabDownloadedTitle, CurrentTabEntity.Name, CurrentTabEntity.Group),
                        new DialogActionContainer
                        {
                            OnTapAction = (o, e) => DoGoToTabView(CurrentTab.Id)
                        });


                    DownloadTab.RaiseCanExecuteChanged();
                });
        }

        void groupImagesSearch_MediaSearchCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            var result = sender as IMediaSearcher;

            CurrentTab.Group.ImageUrl = result.Entry.ImageUrl;
            CurrentTab.Group.LargeImageUrl = result.Entry.LargeImageUrl;
            CurrentTab.Group.ExtraLargeImageUrl = result.Entry.ExtraLargeImageUrl;

            Database.UpdateGroupMediaByName(CurrentTab.Group.Name, result.Entry.ImageUrl,
            result.Entry.LargeImageUrl, result.Entry.ExtraLargeImageUrl);
            //Request A new background imaage
            Hub.RaiseBackGroundImageChangeActivity(CurrentTab.Group.ExtraLargeImageUrl);

        }

        void tabAlbumSearch_MediaSearchCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            var result = sender as IMediaSearcher;
          
            try
            {
                var albumCover = result.Entry.ImageUrl;

                if (!string.IsNullOrEmpty(albumCover))
                {
                    CurrentTab.AlbumCoverImageUrl = result.Entry.ImageUrl;
                    Database.UpdateTabMediaById(CurrentTab.Id, albumCover);
                }
                else
                {
                    if (!String.IsNullOrEmpty(result.Entry.LargeImageUrl))
                        CurrentTab.AlbumCoverImageUrl = result.Entry.LargeImageUrl;
                    else if (!String.IsNullOrEmpty(CurrentTab.Group.ImageUrl))
                        CurrentTab.AlbumCoverImageUrl = CurrentTab.Group.ImageUrl;
                    else
                        CurrentTab.AlbumCoverImageUrl = "";

                    Database.UpdateTabMediaById(CurrentTab.Id, CurrentTab.AlbumCoverImageUrl);
                }
            }
            catch
            {
                //handle catch
            }
               
        }

       
        #endregion Event handlers

        #region Helper methods

        private void CreateCommands()
        {
            GoToTabView = new ExecuteCommand<int>(DoGoToTabView);
            LaunchSearch = new ExecuteCommand<string>(DoLaunchSearch);
            LaunchSearchForBand = new ExecuteCommand<string>(DoLaunchSearchForBand);
            LaunchSearchPopularTabs = new ExecuteCommand<string>(DoLaunchSearchPopularTabs);
            SelectPage = new ExecuteCommand<string>(DoSelectPage);
            DownloadTab = new ExecuteCommand<string>(DoDownloadTab);
            ToggleActionArea = new ExecuteCommand<TabEntity>(DoToggleActionArea);
            NavigatePage = new ExecuteCommand<string>(DoNavigatePage);
        }

        private void RunSearch(string bandName, string songName, ResultsSortOrder sortBy)
        {
            _sortOrder = sortBy;
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                Dialog.Show(AppResources.Search_NoInternetConnection, AppResources.Search_OperationFailed);
                return;
            }

            SearchGroupTabs = null;
            SearchPopularTabs = null;
            IsHintVisible = false;
            IsNothingFound = false;          

            IsSearching = true;
        
            _tabSearcher.Run(bandName, songName, CurrentPageIndex, SearchTabType, sortBy);
        }

        private void ClearSearchArea()
        {
            //There is a fundamenal Architectural Mistake in the project in terms of Search Header hide/show logic in the SearchView
            //If SearchGroupTabs is not initialized after a search is performed - the search header is never visible, therefore a dummy initialization is necessary after an external search.
            SearchGroupTabs = new TabsByName(new ObservableCollection<TabEntity>(new List<TabEntity>()), Database);
            this.CurrentTypedText = this.CurrentSearchText = null;
            HeaderPagingVisibility = Visibility.Collapsed;
        }
        private void AssignHeaderPagingUI(int pageCount)
        {
            PagesListAlignment = HorizontalAlignment.Center;

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