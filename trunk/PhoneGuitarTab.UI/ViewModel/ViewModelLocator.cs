/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:PhoneGuitarTab.UI"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using PhoneGuitarTab.Core;
using PhoneGuitarTab.Core.Navigation;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.UI.Infrastructure;
using PhoneGuitarTab.UI.Infrastructure.Enums;

namespace PhoneGuitarTab.UI.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        private static Core.ViewModel _startup;
        private static Core.ViewModel _collection;
        private static Core.ViewModel _group;
        private static Core.ViewModel _search;
        private static Core.ViewModel _textTab;
        private static Core.ViewModel _help;
        private static Core.ViewModel _about;

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            RegisterAll();

            PageMapping pageMapping = Container.Resolve<PageMapping>();
            _startup = pageMapping.GetViewModel(PageType.Get(ViewType.Startup));
            _collection = pageMapping.GetViewModel(PageType.Get(ViewType.Collection)); 
            _group = pageMapping.GetViewModel(PageType.Get(ViewType.Group));
            _search = pageMapping.GetViewModel(PageType.Get(ViewType.Search));
            _textTab = pageMapping.GetViewModel(PageType.Get(ViewType.TextTab));
            _help = pageMapping.GetViewModel(PageType.Get(ViewType.Help));
            _about = pageMapping.GetViewModel(PageType.Get(ViewType.About));
        }

        /// <summary>
        /// Registers all singleton instances in container
        /// </summary>
        private void RegisterAll()
        {
            //register page mapping
            Container.Register<PageMapping>(new object[]
                                                {
                                                     new Dictionary<IPageType,ObservableTuple<Uri, Lazy<Core.ViewModel>>>()
                                                         {
                                                             {PageType.Get(ViewType.Startup), new ObservableTuple<Uri,Lazy<Core.ViewModel>>(new Uri(@"/View/StartupView.xaml", UriKind.Relative), new Lazy<Core.ViewModel>(()=>new StartupViewModel()))},
                                                             {PageType.Get(ViewType.Collection),new ObservableTuple<Uri,Lazy<Core.ViewModel>>(new Uri(@"/View/CollectionView.xaml", UriKind.Relative), new Lazy<Core.ViewModel>(()=>new CollectionViewModel()))},
                                                             {PageType.Get(ViewType.Search),new ObservableTuple<Uri,Lazy<Core.ViewModel>>(new Uri(@"/View/SearchForBandView.xaml", UriKind.Relative), new Lazy<Core.ViewModel>(()=>new SearchViewModel()))},
                                                             {PageType.Get(ViewType.TextTab),new ObservableTuple<Uri,Lazy<Core.ViewModel>>(new Uri(@"/View/TextTabView.xaml", UriKind.Relative), new Lazy<Core.ViewModel>(()=>new TextTabViewModel()))},
                                                             {PageType.Get(ViewType.Help),new ObservableTuple<Uri,Lazy<Core.ViewModel>>(new Uri(@"/View/HelpView.xaml", UriKind.Relative), new Lazy<Core.ViewModel>(()=>new HelpViewModel()))},
                                                             {PageType.Get(ViewType.About),new ObservableTuple<Uri,Lazy<Core.ViewModel>>(new Uri(@"/View/AboutView.xaml", UriKind.Relative), new Lazy<Core.ViewModel>(()=>new AboutViewModel()))},
                                                             {PageType.Get(ViewType.Group),new ObservableTuple<Uri,Lazy<Core.ViewModel>>(new Uri(@"/View/GroupView.xaml", UriKind.Relative), new Lazy<Core.ViewModel>(()=>new GroupViewModel()))}
                                                         }
                                                });
            
            Container.Register<INavigationService, NavigationService>();
            //TODO read connection string from somewhere
            Action<IDataContextService> initialize = TabDataContextHelper.InitializeDatabase;
            Container.Register<IDataContextService, DataContextService>(new object[]
                                                                            {
                                                                                App.dbConnectionString,
                                                                                initialize
                                                                            });
            Container.Register<IFileSystemService, IsolatedStorageFileService>();
            Container.Register<ISettingService, IsolatedStorageSettingService>();        
        }
     

        #region View models

        /// <summary>
        /// Gets the Main property which defines the main viewmodel.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public PhoneGuitarTab.Core.ViewModel Startup
        {
            get
            {
                return _startup;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public PhoneGuitarTab.Core.ViewModel Collection
        {
            get
            {
                return _collection;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public PhoneGuitarTab.Core.ViewModel Group
        {
            get
            {
                return _group;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public PhoneGuitarTab.Core.ViewModel Search
        {
            get
            {
                return _search;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public PhoneGuitarTab.Core.ViewModel TextTab
        {
            get
            {
                return _textTab;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public PhoneGuitarTab.Core.ViewModel Help
        {
            get
            {
                return _help;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public PhoneGuitarTab.Core.ViewModel About
        {
            get
            {
                return _about;
            }
        }

        #endregion View models
    }
}