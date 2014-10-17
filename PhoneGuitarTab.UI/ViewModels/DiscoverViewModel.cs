using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Primitives;
using PhoneGuitarTab.Core.Views.Commands;
using PhoneGuitarTab.Search.Extensions;
using PhoneGuitarTab.Search.Suggestions;
using PhoneGuitarTab.UI.Data;
using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.Infrastructure;
using PhoneGuitarTab.UI.Resources;

namespace PhoneGuitarTab.UI.ViewModels
{
    public class DiscoverViewModel : DataContextViewModel
    {
        #region  Fields

        private readonly IBandSuggestor _bandSuggestor;

        private ItemsForSuggestion suggestedItems;
        private List<string> baseBands;
        private bool isLoading;
        private bool isRefreshNeeded;
        private bool isBaseBandAvailable;
        private string backGroundImage;
        private string backgroundImageForGenre;
        private bool isSuggestedBandsHeaderAdded;
        #endregion  Fields

        #region Constructors

        [Dependency]
        public DiscoverViewModel( IDataContextService database, MessageHub hub, IBandSuggestor bandSuggestor)
            : base(database, hub)
        {
            _bandSuggestor = bandSuggestor;
            DataBind();
            CreateCommands();
            RegisterEvents();
        }

        #endregion Constructors

        #region Properties

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }

        public bool IsBaseBandAvailable
        {
            get { return isBaseBandAvailable; }
            set
            {
                isBaseBandAvailable = value;
                RaisePropertyChanged("IsBaseBandAvailable");
            }
        }
        public ItemsForSuggestion SuggestedItems
        {
            get { return suggestedItems; }
            set
            {
                suggestedItems = value;
                RaisePropertyChanged("SuggestedItems");
            }
        }

        public string BackGroundImage
        {
            get
            {
                return backGroundImage;
            }
            set
            {
                backGroundImage = value;
                RaisePropertyChanged("BackGroundImage");
            }
        }
        public string BackGroundImageForGenre
        {
            get
            {
                return backgroundImageForGenre;
            }
            set
            {
                backgroundImageForGenre = value;
                RaisePropertyChanged("BackGroundImageForGenre");
            }
        }
        #endregion Properties

        #region Commands
        public ExecuteCommand SearchSuggestions { get; private set; }
        public ExecuteCommand<object> GoTo { get; private set; }
        public ExecuteCommand<object> GoToSuggestedGroup { get; private set; }

        #endregion Commands

        #region CommandHandlers

        private void DoSearchSuggestions()
        {
           
            if (NetworkInterface.GetIsNetworkAvailable() && isRefreshNeeded && IsBaseBandAvailable)
            {
                InitSuggestedBandsHeader();
                this.IsLoading = true;
                this.isRefreshNeeded = false;
                this.SuggestedItems.ClearSuggestedArtists();
                this.baseBands.Clear();
               
               var allBandsInTabDownloadOrder = Database.Tabs.OrderByDescending(t => t.Id).Select(g => g.Group.Name.TransLiterate()).ToList();
               //Add each distinct artist to the basebands list. Note: GroupBy or Distinct clause can not be used here as both of them sorts the grouped list.
                //We need unsorted order of Bands so the 1st element of the list will be the last downloaded tabs artist
               foreach (var artist in allBandsInTabDownloadOrder)
                {
                    if (!baseBands.Contains(artist))
                        baseBands.Add(artist);
                }
             
                _bandSuggestor.RunBandSuggestor(baseBands);                  
              
            }       
           
        }

        private void DoGoToSuggestedGroup(object source)
        {
            Group group;
            if (source.GetType() == typeof(LongListSelector))
                group = (source as LongListSelector).SelectedItem as Group;
            else
                group = source as Group;
            
            //Setting Background image here is necessary because Background image on the next page is not bindable and should be set before navigating to the page.
          
            this.BackGroundImage = group.ExtraLargeImageUrl;
            NavigationService.NavigateTo(NavigationViewNames.SuggestedGroup, new Dictionary<string, object> { { "suggestedgroup", group } }); 
          
        }
        

        private void DoGoToGenre(Group genre)
        {
            //Setting Background image here is necessary because Background image on the next page is not bindable and should be set before navigating to the page.
            this.BackGroundImageForGenre = genre.ExtraLargeImageUrl;
            NavigationService.NavigateTo(NavigationViewNames.GenreGroups, new Dictionary<string, object> { { "genregroups", genre } });

        }

        private void DoGoTo(object sender)
        {
            var itemToDiscover = (sender as LongListSelector).SelectedItem as ObservableTuple<ItemType, Group>;
           
            try
            {
                if (itemToDiscover.Item1 == ItemType.ByArtist)
                    DoGoToSuggestedGroup(itemToDiscover.Item2);   
                else if(itemToDiscover.Item1 == ItemType.ByGenre)
                    DoGoToGenre(itemToDiscover.Item2);

            }
            catch (Exception ex)
            {
                var m = ex.Message;
                throw;
            }
            
        }
        #endregion CommandHandlers

        #region HelperMethods
        private void CreateCommands()
        {
            SearchSuggestions = new ExecuteCommand(DoSearchSuggestions);
            GoTo = new ExecuteCommand<object>(DoGoTo);
            GoToSuggestedGroup = new ExecuteCommand<object>(DoGoToSuggestedGroup);
        }

        private void RegisterEvents()
        {

            Hub.BandSuggestionRequest += (o, args) => DoSearchSuggestions();
            Hub.TabsDownloaded += (o, args) =>
            {
                isRefreshNeeded = true;
                IsBaseBandAvailable = true;
            };
         
            _bandSuggestor.SuggestionSearchCompleted += (s, e) => SuggestionSearchCompleted(s);

        }

        private void FillGenres()
        {   //Insert the Suggestions for you header to the List
            var browseByGenreHeader = new Group { Name = AppResources.Discover_BrowseByGenre};
            this.SuggestedItems.Add(new ObservableTuple<ItemType, Group>(ItemType.CollectionHeader, browseByGenreHeader));

            //Add Genres
            this.SuggestedItems.Add(new ObservableTuple<ItemType, Group>(ItemType.ByGenre,  
                new Group { Name = "ROCK", ExtraLargeImageUrl = "/Images/genres/rock.jpg" }));
            this.SuggestedItems.Add(new ObservableTuple<ItemType, Group>(ItemType.ByGenre,  
                new Group { Name = "ALTERNATIVE", ExtraLargeImageUrl = "/Images/genres/alternative.jpg" }));
            this.SuggestedItems.Add(new ObservableTuple<ItemType, Group>(ItemType.ByGenre,
                new Group { Name = "INDIE", ExtraLargeImageUrl = "/Images/genres/indie.jpg" }));
            this.SuggestedItems.Add(new ObservableTuple<ItemType, Group>(ItemType.ByGenre,
                new Group { Name = "HEAVY METAL", ExtraLargeImageUrl = "/Images/genres/heavymetal.jpg" }));
            this.SuggestedItems.Add(new ObservableTuple<ItemType, Group>(ItemType.ByGenre,
                new Group { Name = "CLASSIC ROCK", ExtraLargeImageUrl = "/Images/genres/classicrock.jpg" }));
            this.SuggestedItems.Add(new ObservableTuple<ItemType, Group>(ItemType.ByGenre,  
                new Group { Name = "90s", ExtraLargeImageUrl = "/Images/genres/90s.jpg" }));
            this.SuggestedItems.Add(new ObservableTuple<ItemType, Group>(ItemType.ByGenre,
                new Group { Name = "JAZZ", ExtraLargeImageUrl = "/Images/genres/jazz.jpg" }));
            this.SuggestedItems.Add(new ObservableTuple<ItemType, Group>(ItemType.ByGenre,
                new Group { Name = "PUNK", ExtraLargeImageUrl = "/Images/genres/punk.jpg" }));
            this.SuggestedItems.Add(new ObservableTuple<ItemType, Group>(ItemType.ByGenre,
                new Group { Name = "POP", ExtraLargeImageUrl = "/Images/genres/pop.jpg" }));
            this.SuggestedItems.Add(new ObservableTuple<ItemType, Group>(ItemType.ByGenre,
               new Group { Name = "COUNTRY", ExtraLargeImageUrl = "/Images/genres/country.jpg" }));
            this.SuggestedItems.Add(new ObservableTuple<ItemType, Group>(ItemType.ByGenre,
               new Group { Name = "LATINO", ExtraLargeImageUrl = "/Images/genres/latino.jpg" }));
        }

        private void InitSuggestedBandsHeader()
        {
            if (!isSuggestedBandsHeaderAdded )
            {
                var suggestionsHeader = new Group { Name = AppResources.Discover_SuggestionsTitle };
                this.SuggestedItems.Add(new ObservableTuple<ItemType, Group>(ItemType.CollectionHeader, suggestionsHeader));
                isSuggestedBandsHeaderAdded = true;
            }
          
            
        }
        #endregion HelperMethods

        #region Override methods

        protected override void DataBind()
        {
            this.SuggestedItems = new ItemsForSuggestion();
            this.FillGenres();
            this.baseBands = new List<string>();
            this.isRefreshNeeded = true;
            this.IsBaseBandAvailable = Database.Groups.Any();
        }

        #endregion

        #region Event Handlers
        void SuggestionSearchCompleted(object sender)
        {

            var suggestions = sender as IBandSuggestor;
            try
            {
                foreach (var band in suggestions.Results)
                {
                    if (this.SuggestedItems.All(b => b.Item2.Name.TransLiterate() != band.BandName.TransLiterate()))
                    {
                        var group = new Group{Name = band.BandName, ExtraLargeImageUrl = band.ExtraLargeImageUrl};
                        this.SuggestedItems.Add(new ObservableTuple<ItemType, Group>(ItemType.ByArtist, group));
                        
                    }
                    
                }
                this.IsLoading = false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion  
               
    }
}