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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using PhoneGuitarTab.Search;
using PhoneGuitarTab.Search.Lastfm;
using PhoneGuitarTab.UI.Notation.Infrastructure;
using PhoneGuitarTab.UI.Notation.Persistence;

namespace PhoneGuitarTab.UI.Notation.ViewModel
{
    public class GroupViewModel : ViewModelBase
    {
        private readonly InteractionRequest<Notification> submitNotificationInteractionRequest;


        public GroupViewModel()
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
            //go to tab view after choosing tab on tab panel
            GoToTabView = new DelegateCommand<string>(DoGoToTabView, CanGoToTabView);
            RemoveTab = new DelegateCommand<string>(DoRemoveTab, CanRemoveTab);
            CancelTab = new DelegateCommand<string>(DoCancelTab, CanCancelTab);
           // ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
           // BackgroundImagePath =
             //   imageSourceConverter.ConvertFromString("/PhoneGuitarTab.UI.Notation;/Images/all/Water.jpg") as ImageSource;

            // BackgroundImagePath =  "/Images/all/Water.jpg";}

            //BackgroundImagePath =
            //    new BitmapImage(new Uri("Images/all/Water.jpg", UriKind.Relative));
        }



        private void DoGoToTabView(string arg)
        {
            NavigationServiceEx nav = new NavigationServiceEx();
            nav.NavigateToWithParams(PageType.Tab, new Dictionary<string, object>()
			{
				{"GroupId", arg}
			});
        }

        private bool CanGoToTabView(string arg)
        {
            return true;
        }

        private bool _isTransitionEnabled = true;

        private void DoRemoveTab(string arg)
        {
            // int id;
            //  if (int.TryParse(arg, out id))
            // {
            RepositoryHelper.RemoveTab(arg);
            Tabs = RepositoryHelper.GetTabs(CurrentGroup.Id);
            //  }
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


        //private ImageSource _backgroundImagePath;
        // public ImageSource BackgroundImagePath
        // {
        //     get { return _backgroundImagePath; }
        //     set
        //     {
        //         if (_backgroundImagePath == value)
        //             return;

        //         _backgroundImagePath = value;

        //         RaisePropertyChanged(() => BackgroundImagePath);
        //     }
        // }

        private Group _currentGroup;
        public Group CurrentGroup
        {
            get { return _currentGroup; }
            set
            {
                if (_currentGroup == value)
                    return;

                _currentGroup = value;

                RaisePropertyChanged(() => CurrentGroup);
            }
        }

        private Tab _selectedGroupTab;
        public Tab SelectedGroupTab
        {
            set
            {
                _selectedGroupTab = value;
                RaisePropertyChanged(() => SelectedGroupTab);
                //NOTE: go to specific group page 
                //Commad would be better
                NavigationServiceEx nav = new NavigationServiceEx();
                nav.NavigateToWithParams(PageMapping<ViewModelBase>
                    .GetPageTypeByTab(_selectedGroupTab), new Dictionary<string, object>()
                                                           {
                                                               {"TabId", _selectedGroupTab.Id}
                                                           });
            }
            get { return _selectedGroupTab; }
        }

        private List<Tab> _tabs;
        public List<Tab> Tabs
        {
            get { return _tabs; }
            set
            {
                if (_tabs == value)
                    return;

                _tabs = value;

                RaisePropertyChanged(() => Tabs);
            }

        }

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

        public DelegateCommand<string> GoToTabView
        {
            get;
            private set;
        }

        public DelegateCommand<string> GoToTabsForGroup
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

            CurrentGroup =
                SterlingService.Current.Database.Load(typeof(Group), NavigationParameters["GroupId"].ToString()) as
                Group;
            Tabs = RepositoryHelper.GetTabs(CurrentGroup.Id);

           /* if (String.IsNullOrEmpty(CurrentGroup.BackgroundUrl))
            {
                SearchImageResult result = new SearchImageResult(CurrentGroup.Name);
                result.SearchComplete += delegate
                                             {
                                                 if ((result.Summary.Total > 0)
                                                     && (result.Entries.Count > 0))
                                                 {
                                                     var format = result.Entries[0].Format;
                                                     if ((format != "png") || (format != "jpg"))
                                                         return;
                                                     var image = result.Entries[0].Sizes.FirstOrDefault(s => s.Name == "original");
                                                     var destination = IsolatedStorageHelper.GetImageFilePath(format);
                                                     FileDownloader downloader = new FileDownloader();
                                                     downloader.DownloadComplete += delegate
                                                                                        {
                                                                                            CurrentGroup.BackgroundUrl =
                                                                                                destination;
                                                                                            RepositoryHelper.Save(CurrentGroup);
                                                                                            Deployment.Current.
                                                                                                Dispatcher.BeginInvoke(
                                                                                                    () =>
                                                                                                        {
                                                                                                            BackgroundImage = destination;
                                                                                                        });
                                                                                           
                                                                                        };
                                                     downloader.Download(image.Url, destination);
                                                 }
                                             };
                result.Run();
            }*/
        }
    }
}
