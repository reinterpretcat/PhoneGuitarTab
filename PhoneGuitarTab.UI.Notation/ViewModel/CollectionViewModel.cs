using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using PhoneGuitarTab.UI.Notation.Infrastructure;
using PhoneGuitarTab.UI.Notation.Persistence;

namespace PhoneGuitarTab.UI.Notation.ViewModel
{
    public class CollectionViewModel : ViewModelBase
    {
        private readonly InteractionRequest<Notification> submitNotificationInteractionRequest;


        public CollectionViewModel()
        {

            submitNotificationInteractionRequest = new InteractionRequest<Notification>();

            SearchCommand =
                new DelegateCommand(
                    () =>
                        {
                            new NavigationServiceEx().NavigateTo(PageType.Search);
                        });

            SettingsCommand =
                new DelegateCommand(
                    () =>
                        {
                            new NavigationServiceEx().NavigateTo(PageType.Settings);
                        });
            HomeCommand =
                new DelegateCommand(
                    () =>
                        {
                            new NavigationServiceEx().NavigateTo(PageType.Startup);
                        });
           
            RemoveTab = new DelegateCommand<string>(DoRemoveTab, CanRemoveTab);
            CancelTab = new DelegateCommand<string>(DoCancelTab, CanCancelTab);

            Update();

        }

        public override void Update()
        {
            base.Update();

            var allGroups = RepositoryHelper.GetAllGroups();
            Groups = new BandByName(allGroups);
            var allTabs = RepositoryHelper.GetAllTabs();
            AllTabs = new TabByName(allTabs);

        }

        private bool _isTransitionEnabled = true;

        private void DoRemoveTab(string arg)
        {
           // int id;
           // if (int.TryParse(arg, out id))
           // {
                RepositoryHelper.RemoveTab(arg);
                AllTabs = new TabByName(RepositoryHelper.GetAllTabs());
                //MessageBox.Show("The tab was removed");
           // }
            _isTransitionEnabled = false;
        }

        private bool CanRemoveTab(string arg)
        {
            //TODO test arg
            return true;
        }

        private void DoCancelTab(string arg)
        {
            _isTransitionEnabled = false;
        }

        private bool CanCancelTab(string arg)
        {
            return true;
        }
       
   
        #region Properties

        private Tuple<int, Group> _selectedGroup;
        public Tuple<int,Group> SelectedGroup
        {
            set
            {
                _selectedGroup = value;
                RaisePropertyChanged(() => SelectedGroup);
                if(_selectedGroup==null)
                    return;
                //NOTE: go to specific group page 
                //Commad would be better
                NavigationServiceEx nav = new NavigationServiceEx();
                nav.NavigateToWithParams(PageType.Group, new Dictionary<string, object>()
                                                             {
                                                                 {"GroupId", _selectedGroup.Item2.Id}
                                                             });
            }
            get { return _selectedGroup; }
        }

        private Tab _selectedTab;
        public Tab SelectedTab
        {
            set
            {
                _selectedTab = value;
                RaisePropertyChanged(() => SelectedTab);
                if (_selectedTab == null)
                    return;
                //NOTE: go to specific group page 
                //Commad would be better
                if (_isTransitionEnabled)
                {
                    NavigationServiceEx nav = new NavigationServiceEx();
                    nav.NavigateToWithParams(PageType.Tab, new Dictionary<string, object>()
                                                               {
                                                                   {"TabId", _selectedTab.Id}
                                                               });
                }
                else
                    _isTransitionEnabled = true;
            }
            get { return _selectedTab; }
        }

        private BandByName _groups;
        public BandByName Groups
        {
            set
            {
                _groups = value;
                RaisePropertyChanged(() => Groups);
            }
            get { return _groups; }
        }

        private TabByName _allTabs;
        public TabByName AllTabs
        {
            set
            {
                _allTabs = value;
                RaisePropertyChanged(() => AllTabs);
            }
            get { return _allTabs; }
            
        }

        /*public DelegateCommand<string> GoToTabView
        {
            get;
            private set;
        }*/

        public DelegateCommand<string> RemoveTab
        {
            get;
            private set;
        }

        public DelegateCommand<string> CancelTab
        {
            get;
            private set;
        }

        public IInteractionRequest SubmitNotificationInteractionRequest
        {
            get
            {
                return this.submitNotificationInteractionRequest;
            }
        }

        public DelegateCommand SearchCommand
        {
            get;
            set;
        }

        public DelegateCommand SettingsCommand
        {
            get;
            set;
        }

        public DelegateCommand HomeCommand
        {
            get;
            set;
        }
        #endregion

        protected override void ReadNavigationParameters()
        {
        }
    }
}
