
using PhoneGuitarTab.Core.Cloud;
using PhoneGuitarTab.Core.Dependencies;

namespace PhoneGuitarTab.UI.Bootstrap
{
    /// <summary>
    /// Handles cloud-related functionallity
    /// </summary>
    public class CloudBootstrapperPlugin
    {
        public const string SkyDriveDirectoryName = "PhoneGuitarTab";
        public const string SkyDriveAppId = "000000004807241A";

        public string Name { get { return "Core"; } }

        [Dependency]
        private IContainer Container { get; set; }

        public bool Run()
        {

            Container.RegisterInstance<ICloudService>(new SkyDriveService(SkyDriveAppId, SkyDriveDirectoryName));
            
            return true;
        }

        public bool Update()
        {
            return true;
        }
    }
}
