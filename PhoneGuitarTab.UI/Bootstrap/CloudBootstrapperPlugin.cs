using PhoneGuitarTab.Core.Bootstrap;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Services;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.Bootstrap
{
    /// <summary>
    ///     Handles cloud-related functionallity
    /// </summary>
    public class CloudBootstrapperPlugin : IBootstrapperPlugin
    {
        private readonly SkyDriveService.SkyDriveAppContext _context = new SkyDriveService.SkyDriveAppContext
        {
            AppFolder = "PhoneGuitarTab",
            ClientId = "000000004807241A"
        };

        public string Name
        {
            get { return "Cloud"; }
        }

        [Dependency]
        private IContainer Container { get; set; }

        public bool Run()
        {
            // NOTE missing functionality for injecting of primitive types in DI container
            // so use and register artificial context type
            Container.RegisterInstance(_context);
            Container.Register(Component.For<ICloudService>().Use<SkyDriveService>().Singleton());
            Container.Register(Component.For<TabSyncService>().Use<TabSyncService>().Singleton());

            return true;
        }

        public bool Update()
        {
            return true;
        }
    }
}