namespace PhoneGuitarTab.Core.Bootstrap
{
    /// <summary>
    /// Represents a startup plugin
    /// </summary>
    public interface IBootstrapperPlugin
    {
        /// <summary>
        /// The name of plugin
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Run plugin
        /// </summary>
        bool Run();

        /// <summary>
        /// Update plugin
        /// </summary>
        bool Update();
    }
}
