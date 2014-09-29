using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using Microsoft.Phone.Tasks;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Views.Commands;
using PhoneGuitarTab.Search;
using PhoneGuitarTab.Search.Arts;
using PhoneGuitarTab.UI.Data;
using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.ViewModels
{
    public class SuggestedGroupViewModel : DataContextViewModel
    {
        #region  Fields

        private readonly IMediaSearcherFactory _mediaSearcherFactory;
        private Group _currentGroup;
        private string _summary;
        private string _imageUrl;
        private string _largeImageUrl;
        private string _extraLargeImageUrl;
        private TabsForBand _tabs;

        private bool isLoading;
        private bool infoFound;

        #endregion  Fields

        #region Constructors

        [Dependency]
        public SuggestedGroupViewModel(IMediaSearcherFactory mediaSearcherFactory, IDataContextService database, MessageHub hub)
            : base(database, hub)
        {
            _mediaSearcherFactory = mediaSearcherFactory;
            CreateCommands();
        }

        #endregion Constructors

        #region Properties

        public Group CurrentGroup
        {
            get { return _currentGroup; }
            set
            {
                _currentGroup = value;
                RaisePropertyChanged("CurrentGroup");
                DataBind();
            }
        }

        public string Summary
        {
            get { return _summary; }
            set
            {
                _summary = value;
                RaisePropertyChanged("Summary");
            }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                _imageUrl = value;
                RaisePropertyChanged("ImageUrl");
            }
        }

        public string LargeImageUrl
        {
            get { return _largeImageUrl; }
            set
            {
                _largeImageUrl = value;
                RaisePropertyChanged("LargeImageUrl");
            }
        }

        public string ExtraLargeImageUrl
        {
            get { return _extraLargeImageUrl; }
            set
            {
                _extraLargeImageUrl = value;
                RaisePropertyChanged("ExtraLargeImageUrl");
            }
        }

        public TabsForBand Tabs
        {
            get { return _tabs; }
            set
            {
                _tabs = value;
                RaisePropertyChanged("Tabs");
            }
        }

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

        #endregion Properties

        #region Override members

        public override void SaveStateTo(IDictionary<string, object> state)
        {
            base.SaveStateTo(state);
            state["CurrentGroupId"] = CurrentGroup.Id;
        }

        protected override void DataBind()
        {
            if (CurrentGroup != null)
            {
                Tabs = new TabsForBand(CurrentGroup.Id, Database);

                NothingFound = false;

                if (String.IsNullOrEmpty(CurrentGroup.Description))
                {
                    GetCurrentGroupInfo(CurrentGroup);
                }
                else
                {
                    Summary = CurrentGroup.Description;
                    ImageUrl = CurrentGroup.ImageUrl;
                    LargeImageUrl = CurrentGroup.LargeImageUrl;
                    ExtraLargeImageUrl = CurrentGroup.ExtraLargeImageUrl;
                }
            }
        }

        protected override void ReadNavigationParameters()
        {
            if (NavigationParameters == null)
                return;
            CurrentGroup = (Group) NavigationParameters["suggestedgroup"];
        }

        #endregion Override members

        #region Commands

        public ExecuteCommand<int> RemoveTab { get; private set; }

        public ExecuteCommand CancelTab { get; private set; }

        public ExecuteCommand<object> GoToTabView { get; private set; }

        public ExecuteCommand<Group> SearchCommand { get; set; }

        public ExecuteCommand<int> PinTabToStart { get; private set; }

        public ExecuteCommand HomeCommand { get; set; }

        public ExecuteCommand GetMoreInfo
        {
            get
            {
                return new ExecuteCommand(() =>
                    new WebBrowserTask {URL = CurrentGroup.Url}.Show());
            }
        }

        public ExecuteCommand<Group> RefreshInfoCommand { get; set; }

        #endregion Commands

        #region Command handlers

        private void DoGoToTabView(object args)
        {
            var tabEntity = (args as TabEntity);
            if (tabEntity != null)
            {
                Tab tab = (from Tab t in Database.Tabs
                    where t.Id == tabEntity.Id
                    select t).Single();
                NavigationService.NavigateToTab(new Dictionary<string, object> {{"Tab", tab}});
                Hub.RaiseTabBrowsed();
                Hub.RaiseTabsRefreshed();
            }
        }

        private void DoRemoveTab(int id)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Database.DeleteTabById(id);
                DataBind();
            });
            Hub.RaiseGroupTabRemoved(id);
        }

        private void DoSearch(Group group)
        {
            NavigationService.NavigateTo(NavigationViewNames.Search, new Dictionary<string, object> {{"SearchTerm", group.Name}});
        }

        private void DoRefreshInfo(Group group)
        {
            GetCurrentGroupInfo(group);
        }

        private void DoHome()
        {
            NavigationService.NavigateTo(NavigationViewNames.Startup);
        }

        private void DoPinTabToStart(int id)
        {
            Tab tab = (from Tab t in Database.Tabs
                where t.Id == id
                select t).Single();

            TilesForTabs.PinTabToStart(tab);
        }

        #endregion Command handlers

        #region Event handlers


        #endregion Event handlers

        #region Helper methods

        private void CreateCommands()
        {
            SearchCommand = new ExecuteCommand<Group>(DoSearch);
            HomeCommand = new ExecuteCommand(DoHome);
            RefreshInfoCommand = new ExecuteCommand<Group>(DoRefreshInfo);
            PinTabToStart = new ExecuteCommand<int>(DoPinTabToStart);
            GoToTabView = new ExecuteCommand<object>(DoGoToTabView);
            RemoveTab = new ExecuteCommand<int>(DoRemoveTab);
        }

        private void GetCurrentGroupInfo(Group group)
        {
            var bandInfoSearch = _mediaSearcherFactory.Create();
            bandInfoSearch.MediaSearchCompleted += bandInfoSearch_MediaSearchCompleted;
            bandInfoSearch.RunMediaSearch(group.Name, string.Empty);        
            IsLoading = true;
            NothingFound = false;

        }

        void bandInfoSearch_MediaSearchCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var result = sender as IMediaSearcher;
            try
            {
                var description = result.Entry.Summary;

                if (!string.IsNullOrEmpty(description))
                {
                    if (description.Length > 2040)
                    {
                        description = description.Substring(0, 2080);
                        description += "..";
                    }
                    Summary = description;
                    CurrentGroup.Description = Summary;
                    CurrentGroup.Url = result.Entry.Url;
                    CurrentGroup.ImageUrl = result.Entry.ImageUrl;
                    CurrentGroup.LargeImageUrl = result.Entry.LargeImageUrl;
                    CurrentGroup.ExtraLargeImageUrl = result.Entry.ExtraLargeImageUrl;
                    Database.SubmitChanges();
                }
                else
                {
                    NothingFound = true;
                }
            }
            catch
            {
                // buried intentionally
            }
            finally
            {
                IsLoading = false;
            }
           
        }

        #endregion Helper methods
    }
}