namespace PhoneGuitarTab.UI.Bootstrap
{
    using System.Linq;
    using PhoneGuitarTab.Core.Bootstrap;
    using PhoneGuitarTab.Core.Dependencies;

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
            _container.Register(Component.For<IBootstrapperPlugin>().Use<TabBootstraperPlugin>().Named("Tab").Singleton());

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
        public PhoneGuitarTab.Core.ViewModel Startup
        {
            get
            {
                return _container.Resolve<Core.ViewModel>(Strings.Startup);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public PhoneGuitarTab.Core.ViewModel Group
        {
            get
            {
                return _container.Resolve<Core.ViewModel>(Strings.Group);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public PhoneGuitarTab.Core.ViewModel Search
        {
            get
            {
                return _container.Resolve<Core.ViewModel>(Strings.Search);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public PhoneGuitarTab.Core.ViewModel TextTab
        {
            get
            {
                return _container.Resolve<Core.ViewModel>(Strings.TextTab);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public PhoneGuitarTab.Core.ViewModel Help
        {
            get
            {
                return _container.Resolve<Core.ViewModel>(Strings.Help);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public PhoneGuitarTab.Core.ViewModel About
        {
            get
            {
                return _container.Resolve<Core.ViewModel>(Strings.About);
            }
        }

        #endregion View models
      
    }
}
