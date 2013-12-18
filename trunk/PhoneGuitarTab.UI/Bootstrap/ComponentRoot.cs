namespace PhoneGuitarTab.UI.Bootstrap
{
    using System.Linq;
    using PhoneGuitarTab.Core.Bootstrap;
    using PhoneGuitarTab.Core.Dependencies;
    using PhoneGuitarTab.Core.Views;
    using PhoneGuitarTab.UI.ViewModel;

    /// <summary>
    /// Represents an application bootstrapper.
    /// NOTE I prefer here "Code over Configuration" approach
    /// </summary>
    public class ComponentRoot
    {
        private IContainer _container;

        public ComponentRoot()
        {
            _container = new Container();

            _container.Register(Component.For<IBootstrapperPlugin>().Use<CoreBootstrapperPlugin>().Named("Core").Singleton());
            _container.Register(Component.For<IBootstrapperPlugin>().Use<DataBootstrapperPlugin>().Named("Data").Singleton());
            _container.Register(Component.For<IBootstrapperPlugin>().Use<NavigationBootstrapperPlugin>().Named("Navigation").Singleton());
            _container.Register(Component.For<IBootstrapperPlugin>().Use<CloudBootstrapperPlugin>().Named("Cloud").Singleton());

            _container.ResolveAll<IBootstrapperPlugin>().ToList()
                .Aggregate(true, (current, task) => current & task.Run());
        }

        #region View models

        /// <summary>
        /// Gets the Main property which defines the main viewmodel.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public StartupViewModel Startup
        {
            get
            {
                return _container.Resolve<ViewModel>(Strings.Startup) as StartupViewModel;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public GroupViewModel Group
        {
            get
            {
                return _container.Resolve<ViewModel>(Strings.Group) as GroupViewModel;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public SearchViewModel Search
        {
            get
            {
                return _container.Resolve<ViewModel>(Strings.Search) as SearchViewModel;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public TextTabViewModel TextTab
        {
            get
            {
                return _container.Resolve<ViewModel>(Strings.TextTab) as TextTabViewModel;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public HelpViewModel Help
        {
            get
            {
                return _container.Resolve<ViewModel>(Strings.Help) as HelpViewModel;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public AboutViewModel About
        {
            get
            {
                return _container.Resolve<ViewModel>(Strings.About) as AboutViewModel;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public CollectionViewModel Collection
        {
            get
            {
                return _container.Resolve<ViewModel>(Strings.Collection) as CollectionViewModel;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
         "CA1822:MarkMembersAsStatic",
         Justification = "This non-static member is needed for data binding purposes.")]
        public StaveTabViewModel StaveTab
        {
            get
            {
                return _container.Resolve<ViewModel>(Strings.StaveTab) as StaveTabViewModel;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic",
        Justification = "This non-static member is needed for data binding purposes.")]
        public SynchronizeViewModel Synchronize
        {
            get
            {
                return _container.Resolve<ViewModel>(Strings.Synchronize) as SynchronizeViewModel;
            }
        }

        #endregion View models
      
    }
}
