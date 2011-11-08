using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.UI.Controls;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.ViewModel
{
    public class CollectionViewModel : PhoneGuitarTab.Core.ViewModel
    {
        //private readonly InteractionRequest<Notification> submitNotificationInteractionRequest;

        public CollectionViewModel()
        {
            SearchCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(PageType.EnumType.Search)));
            SettingsCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(PageType.EnumType.Settings)));
            HomeCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.Get(PageType.EnumType.Startup)));

            RemoveTab = new RelayCommand<int>(DoRemoveTab);
            CancelTab = new RelayCommand(() => { });

            GoToGroup = new RelayCommand<object>(DoGoToGroup);
            GoToTabView = new RelayCommand<object>(DoGoToTabView);

            Initialize();
           
        }

        private void Initialize()
        {
            //get group list
            Groups = new BandByName();
            RaisePropertyChanged("Groups");
            //get songs list
            AllTabs = new TabByName();
            RaisePropertyChanged("AllTabs");
     
        }

        public RelayCommand<object> GoToGroup
        {
            get;
            private set;
        }

        private void DoGoToGroup(object sender)
        {       
            var selector = sender as Microsoft.Phone.Controls.LongListSelector;
            Group group = (selector.SelectedItem as Tuple<int,Group>).Item2;
            navigationService.NavigateTo(PageType.Get(PageType.EnumType.Group), new Dictionary<string,object> {{"group", group}});
        }

        private void DoGoToTabView(object sender)
        {
            //e.AddedItems[0] as Microsoft.Phone.Controls.LongListSelector.ItemTuple
            Microsoft.Phone.Controls.LongListSelector selector = sender as Microsoft.Phone.Controls.LongListSelector;
            Tab tab = selector.SelectedItem as Tab;
            navigationService.NavigateTo(PageType.Get(PageType.EnumType.TextTab), new Dictionary<string,object>()
                                                                        {
                                                                            {"Tab", tab}
                                                                        });
        }

        private void DoRemoveTab(int id)
        {
            TabDataContextHelper.DeleteTabById(id);
            Initialize();
        }

        #region Properties

        public Tuple<int, Group> SelectedGroup { get; set; }
        public Tab SelectedTab { get; set; }
        public BandByName Groups { get; set; }
        public TabByName AllTabs { get; set; }

        public RelayCommand<object> GoToTabView
        {
            get;
            private set;
        }

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


        public RelayCommand SearchCommand
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
        #endregion

        public override void SaveStateTo(IDictionary<string, object> state)
        {
            
        }

        public override void LoadStateFrom(IDictionary<string, object> state)
        {
           
        }

    }
}
