using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
namespace PhoneGuitarTab.UI.Data
{
    public class AppSettings : PhoneGuitarTab.Core.Services.IsolatedStorageSettingService
    {
            // The key names of our settings
            public const string isAppRatedKeyName = "isApprated";
            public const string tabViewCountKeyName = "tabViewCount";

            // The default value of our settings
            public const bool isAppratedDefault = false;
            public const int tabViewCountDefault = 0;

    }
}
