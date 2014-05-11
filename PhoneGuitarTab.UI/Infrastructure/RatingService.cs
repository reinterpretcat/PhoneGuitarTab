using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Services;

namespace PhoneGuitarTab.UI.Infrastructure
{
    public class RatingService
    {
        private const string IsAppRatedKeyName = "isApprated";
        private const string TabViewCountKeyName = "tabViewCount";

        private const bool IsAppratedDefault = false;
        private const int TabViewCountDefault = 0;
        private const int ShowRatingInEveryN = 7;

        private readonly ISettingService _settingService;

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

        public bool IsNeedShowMessage()
        {
            return (_settingService.GetValueOrDefault(TabViewCountKeyName, TabViewCountDefault)%ShowRatingInEveryN) == 0;
        }

        public void IncreaseTabViewCount()
        {
            int tabCount = _settingService.GetValueOrDefault(TabViewCountKeyName, TabViewCountDefault);

            if (_settingService.AddOrUpdateValue(TabViewCountKeyName, (tabCount + 1)))
                _settingService.Save();
        }

        public bool IsAppRated()
        {
            return _settingService.GetValueOrDefault(IsAppRatedKeyName, IsAppratedDefault);
        }
    }
}