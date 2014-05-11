using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Diagnostic;

namespace PhoneGuitarTab.Core.Bootstrap
{
    /// <summary>
    ///     Represents a bootstrapper plugin
    /// </summary>
    public abstract class BootstrapperPlugin : IBootstrapperPlugin
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