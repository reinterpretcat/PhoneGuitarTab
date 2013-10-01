namespace PhoneGuitarTab.UI.Bootstrap
{
    using PhoneGuitarTab.Core.Bootstrap;
    using PhoneGuitarTab.Core.Dependencies;
    using PhoneGuitarTab.Core.Diagnostic;
    using PhoneGuitarTab.Core.IsolatedStorage;
    using PhoneGuitarTab.UI.Infrastructure;

    public class CoreBootstrapperPlugin : IBootstrapperPlugin
    {
        public string Name { get { return "Core"; } }

        [Dependency]
        private IContainer Container { get; set; }

        public bool Run()
        {
            Container.Register(Component.For<ITrace>().Use<DefaultTrace>().Singleton());
            Container.Register(Component.For<TraceCategory>().Use<TraceCategory>("Default").Singleton()); // TODO create several named categories

            Container.Register(Component.For<IFileSystemService>().Use<IsolatedStorageFileService>().Singleton());
            Container.Register(Component.For<TabFileStorage>().Use<TabFileStorage>().Singleton());

            Container.Register(Component.For<MessageHub>().Use<MessageHub>().Singleton());

            Container.Register(Component.For<IDialogController>().Use<DefaultDialogController>().Singleton());
            
            return true;
        }

        public bool Update()
        {
            return true;
        }
    }
}
