
namespace PhoneGuitarTab.UI.Data
{
    public class AppSettingService : PhoneGuitarTab.Core.Services.IsolatedStorageSettingService
    {
            // The key names of our settings
            public const string isAppRatedKeyName = "isApprated";
            public const string tabViewCountKeyName = "tabViewCount";

            // The default value of our settings
            public const bool isAppratedDefault = false;
            public const int tabViewCountDefault = 0;

    }
}
