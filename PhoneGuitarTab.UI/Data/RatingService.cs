
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Services;

namespace PhoneGuitarTab.UI.Data
{
    public class RatingService 
    {
        // The key names of our settings
        private const string IsAppRatedKeyName = "isApprated";
        private const string TabViewCountKeyName = "tabViewCount";

        // The default value of our settings
        private const bool IsAppratedDefault = false;
        private const int TabViewCountDefault = 0;

        private readonly  ISettingService _settingService;

        [Dependency]
        public RatingService(ISettingService settingService)
        {
            _settingService = settingService;
        }

        public void RateApp()
        {
            if (_settingService.AddOrUpdateValue(IsAppRatedKeyName, true))
                _settingService.Save();
        }

        public int GetTabViewCountMod()
        {
            return _settingService.GetValueOrDefault<int>(TabViewCountKeyName, TabViewCountDefault) % 4;
        }

        public void IncreaseTabViewCount()
        {
            int tabCount = _settingService.GetValueOrDefault<int>(TabViewCountKeyName, TabViewCountDefault);

            if (this._settingService.AddOrUpdateValue(TabViewCountKeyName, (tabCount + 1)))
                this._settingService.Save();
        }

        public bool IsAppRated()
        {
            return _settingService.GetValueOrDefault<bool>(IsAppRatedKeyName, IsAppratedDefault);
        }

    }
}
