namespace PhoneGuitarTab.Core.Bootstrap
{
    using PhoneGuitarTab.Core.Diagnostic;
    using PhoneGuitarTab.Core.Dependencies;

    /// <summary>
    /// Represents a bootstrapper plugin
    /// </summary>
    public abstract class BootstrapperPlugin: IBootstrapperPlugin
    {
        [Dependency]
        public IContainer Container { get; set; }

        public string Name { get; private set; }

        protected BootstrapperPlugin(string name)
        {
            Name = name;
        }

        [Dependency]
        public ITrace Trace { get; set; }

        [Dependency("Bootstrapping.Plugin")]
        public TraceCategory Category { get; set; }

        #region IBootstrapperPlugin members

        public abstract bool Run();
        public abstract bool Update();

        #endregion
    }
}
