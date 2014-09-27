using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
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
        private bool infoFound;

        #endregion  Fields

        #region Constructors

        [Dependency]
        public DiscoverViewModel( IDataContextService database, MessageHub hub, IBandSuggestor bandSuggestor)
            : base(database, hub)
        {
            _bandSuggestor = bandSuggestor;
            DataBind();
            CreateCommands();
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
                RaisePropertyChanged("InfoLoaded");
            }
        }

        public bool InfoLoaded
        {
            get { return !IsLoading && infoFound; }
        }

        public bool NothingFound
        {
            get { return !infoFound; }
            set
            {
                infoFound = !value;
                RaisePropertyChanged("NothingFound");
                RaisePropertyChanged("InfoLoaded");
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
        #endregion Properties

        #region Commands
        public ExecuteCommand SearchSuggestions { get; private set; }

        private void CreateCommands()
        {
           SearchSuggestions = new ExecuteCommand (DoSearchSuggestions);
        }
        #endregion Commands

        private void DoSearchSuggestions()
        {
            this.IsLoading = true;
            var baseBands = Database.Groups.OrderByDescending(g => g.Id).Select(g => g.Name).ToList();
            _bandSuggestor.RunBandSuggestor(baseBands);
            _bandSuggestor.SuggestionSearchCompleted+=_bandSuggestor_SuggestionSearchCompleted;
        }

        protected override void DataBind()
        {
            this.SuggestedGroups = new BandBySuggestion();        
        }

        void _bandSuggestor_SuggestionSearchCompleted(object sender, DownloadStringCompletedEventArgs e)
            {
 	
                 var suggestion = sender as IBandSuggestor;
                try
                {
                    foreach (var band in suggestion.Results)
                    {
                        var group = new Group();
                        group.Name = band.BandName;
                        group.ExtraLargeImageUrl = band.ExtraLargeImageUrl;
                        this.SuggestedGroups.Add(group);

                    }
                    this.IsLoading = false;
                }
                catch (Exception)
                {
        
                    throw;
                }
            }

      

        
    }
}