using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Tasks;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Views.Commands;
using PhoneGuitarTab.Search.Suggestions;
using PhoneGuitarTab.Search.Arts;
using PhoneGuitarTab.UI.Data;
using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.ViewModels
{
    public class DiscoverViewModel : DataContextViewModel
    {
        #region  Fields

        private readonly IBandSuggestor _bandSuggestor;

        private BandBySuggestion suggestedGroups;
        private bool isLoading;
        private bool isRefreshNeeded;
        private bool isBaseBandAvailable;
        private string backGroundImage;
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
        public BandBySuggestion SuggestedGroups
        {
            get { return suggestedGroups; }
            set
            {
                suggestedGroups = value;
                RaisePropertyChanged("SuggestedGroups");
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
        #endregion Properties

        #region Commands
        public ExecuteCommand SearchSuggestions { get; private set; }
        public ExecuteCommand<object> GoToSuggestedGroup { get; private set; }

        #endregion Commands

        #region CommandHandlers

        private void DoSearchSuggestions()
        {
            if (NetworkInterface.GetIsNetworkAvailable() && isRefreshNeeded && IsBaseBandAvailable)
            {
                this.IsLoading = true;
                this.isRefreshNeeded = false;
                this.SuggestedGroups.Clear();
                var baseBands = Database.Groups.OrderByDescending(g => g.Id).Select(g => g.Name).ToList();
                _bandSuggestor.RunBandSuggestor(baseBands);
                _bandSuggestor.SuggestionSearchCompleted += _bandSuggestor_SuggestionSearchCompleted;
            }
           
        }

        private void DoGoToSuggestedGroup(object sender)
        {
            var selector = sender as Microsoft.Phone.Controls.LongListSelector;
            try
            {
                Group group = selector.SelectedItem as Group;
                this.BackGroundImage = group.ExtraLargeImageUrl;
                Hub.RaiseBackGroundImageChangeActivity(group.ExtraLargeImageUrl);
                NavigationService.NavigateTo(NavigationViewNames.SuggestedGroup, new Dictionary<string, object> { { "suggestedgroup", group } });
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
            GoToSuggestedGroup = new ExecuteCommand<object>(DoGoToSuggestedGroup);
        }

        private void RegisterEvents()
        {

            Hub.BandSuggestionRequest += (o, args) => DoSearchSuggestions();
            Hub.BandCreated += (o, args) =>
            {
                isRefreshNeeded = true;
                IsBaseBandAvailable = true;
            };

        }
        #endregion HelperMethods

        #region Override methods

        protected override void DataBind()
        {
            this.SuggestedGroups = new BandBySuggestion();
            this.isRefreshNeeded = true;
            this.IsBaseBandAvailable = Database.Groups.Any();
        }

        #endregion

        #region Event Handlers
        void _bandSuggestor_SuggestionSearchCompleted(object sender, DownloadStringCompletedEventArgs e)
        {

            var suggestion = sender as IBandSuggestor;
            try
            {
                foreach (var band in suggestion.Results)
                {
                    if (this.SuggestedGroups.All(b => b.Name != band.BandName))
                    {
                        var group = new Group();
                        group.Name = band.BandName;
                        group.ExtraLargeImageUrl = band.ExtraLargeImageUrl;
                        this.SuggestedGroups.Add(group);
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