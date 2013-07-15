using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.Search.Lastfm;
using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.Infrastructure;
using PhoneGuitarTab.UI.Infrastructure.Enums;
using PhoneGuitarTab.UI.Infrastructure.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using Group = PhoneGuitarTab.Data.Group;


namespace PhoneGuitarTab.UI.ViewModel
{
    public class GroupViewModel : DataContextViewModel
    {
        #region  Fields

        private Group _currentGroup;
        private string _summary;
        private TabsForBand _tabs;

        private SearchInfoResult result;

        private bool isLoading = false;
        private bool infoFound = false;

        #endregion  Fields


        #region Constructors

        public GroupViewModel()
        {
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
            get 
            { 
                return isLoading; 
            }
            set
            {
                isLoading = value;
                RaisePropertyChanged("IsLoading");
                RaisePropertyChanged("InfoLoaded");
            }
        }

        public bool InfoLoaded
        {
            get
            {
                return !IsLoading && infoFound;
            }
        }

        public bool NothingFound
        {
            get
            {
                return !infoFound;
            }
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

        public override void LoadStateFrom(IDictionary<string, object> state)
        {
            base.LoadStateFrom(state);
            if (state.ContainsKey("CurrentGroupId"))
            {
                int currentGroupId = (int)state["CurrentGroupId"];
            }

        }

        protected override void DataBind()
        {
            if (CurrentGroup != null)
            {
                Tabs = new TabsForBand(CurrentGroup.Id);

                NothingFound = false;

                if (String.IsNullOrEmpty(CurrentGroup.Description))
                {
                    GetCurrentGroupInfo(CurrentGroup);
                }
                else
                {
                    Summary = CurrentGroup.Description;
                }
            }
        }

        protected override void ReadNavigationParameters()
        {
            if (NavigationParameters == null)
                return;
            CurrentGroup = (Group)NavigationParameters["group"];
        }

        #endregion Override members


        #region Commands

        public RelayCommand<int> RemoveTab
        {
            get;
            private set;
        }

        public RelayCommand CancelTab
        {
            get;
            private set;
        }

        public RelayCommand<object> GoToTabView
        {
            get;
            private set;
        }

        public RelayCommand<Group> SearchCommand
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

        public RelayCommand GetMoreInfo
        {
            get
            {
                return new RelayCommand(() =>
                  new WebBrowserTask { URL = CurrentGroup.Url }.Show());
            }
        }

        public RelayCommand<Group> RefreshInfoCommand
        {
            get;
            set;
        }

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
                navigationService.NavigateTo(PageType.Get(ViewType.TextTab), new Dictionary<string, object>()
                                                                                          {
                                                                                              {"Tab", tab}
                                                                                          });
            }
        }

        private void DoRemoveTab(int id)
        {
            TabDataContextHelper.DeleteTabById(id);

            RemoveTabFromList(id);

            MessengerInstance.Send<GroupTabRemovedMessage>(new GroupTabRemovedMessage(id));
        }

        private void DoSearch(Group group)
        {
            navigationService.NavigateTo(PageType.Get(ViewType.Search), new Dictionary<string, object>() { {"SearchTerm", group.Name} });
        }

        private void DoRefreshInfo(Group group)
        {
            GetCurrentGroupInfo(group);
        }

        #endregion Command handlers


        #region Event handlers

        private void SearchCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var description = result.Summary;

                if (!string.IsNullOrEmpty(description))
                {
                    if (description.Length > 2040)
                    {
                        description = description.Substring(0, 2080);
                        description += "..";
                    }
                    Summary = description;
                    CurrentGroup.Description = Summary;
                    CurrentGroup.Url = result.Url;

                    NothingFound = false;

                    Database.SubmitChanges();
                }
                else
                {
                    NothingFound = true;
                    // TODO: show user that nothing found
                }
            }
            catch (Exception) 
            {
                NothingFound = true;
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion Event handlers


        #region Helper methods

        private void CreateCommands()
        {
            SearchCommand = new RelayCommand<Group>(DoSearch);
            SettingsCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(ViewType.Settings)));
            HomeCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(ViewType.Startup)));
            RefreshInfoCommand = new RelayCommand<Group>(DoRefreshInfo);

            GoToTabView = new RelayCommand<object>(DoGoToTabView);
            RemoveTab = new RelayCommand<int>(DoRemoveTab);
            CancelTab = new RelayCommand(() => { });
        }

        private void GetCurrentGroupInfo(Group group)
        {
            result = new SearchInfoResult(group.Name);
            result.SearchCompleted += SearchCompleted;

            IsLoading = true;
            result.Run();
        }

        private void RemoveTabFromList(int id)
        {
            Tabs.RemoveTab(Tabs.Tabs.Where(tab => tab.Id == id).Single());
        }

        #endregion Helper methods
    }
}
