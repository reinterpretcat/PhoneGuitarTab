using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Services;

namespace PhoneGuitarTab.UI.Infrastructure
{
    public class PopUpMessageService
    {
        private const string IsAppRatedKeyName = "isApprated";
        private const string TabViewCountKeyName = "tabViewCount";

        private const bool IsAppratedDefault = false;
        private const int TabViewCountDefault = 0;
        private const int ShowRatingInEveryN = 7;

        private const string TabViewCountForProMessageKeyName = "tabViewCountForProMessage";
        private const int ShowProMessageAt = 25;
        private readonly ISettingService _settingService;

        [Dependency]
        public PopUpMessageService(ISettingService settingService)
        {
            _settingService = settingService;
        }

        public void RateApp()
        {
            if (_settingService.AddOrUpdateValue(IsAppRatedKeyName, true))
                _settingService.Save();
        }

        public bool IsNeedShowRatingMessage()
        {
            return (_settingService.GetValueOrDefault(TabViewCountKeyName, TabViewCountDefault)%ShowRatingInEveryN) == 0;
        }

        public bool IsNeedShowPurchaseProMessage()
        {
            return _settingService.GetValueOrDefault(TabViewCountForProMessageKeyName, TabViewCountDefault) == ShowProMessageAt;
        }

        public void IncreaseTabViewCount()
        {
            int tabCount = _settingService.GetValueOrDefault(TabViewCountKeyName, TabViewCountDefault);

            if (_settingService.AddOrUpdateValue(TabViewCountKeyName, (tabCount + 1)))
                _settingService.Save();

            int tabCountPro = _settingService.GetValueOrDefault(TabViewCountForProMessageKeyName, TabViewCountDefault);

            if (_settingService.AddOrUpdateValue(TabViewCountForProMessageKeyName, (tabCountPro + 1)))
                _settingService.Save();
        }

        public bool IsAppRated()
        {
            return _settingService.GetValueOrDefault(IsAppRatedKeyName, IsAppratedDefault);
        }
    }
}