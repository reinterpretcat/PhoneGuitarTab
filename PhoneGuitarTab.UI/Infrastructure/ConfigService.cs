using PhoneGuitarTab.Core.Dependencies;

namespace PhoneGuitarTab.UI.Infrastructure
{
    public class ConfigService
    {

        public bool AdEnabled { get; set; }

        [Dependency]
        public ConfigService()
        {
            AdEnabled = true;
        }
     
    }
}