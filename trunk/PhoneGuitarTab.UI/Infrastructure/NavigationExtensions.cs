
using PhoneGuitarTab.Core.Services;

namespace PhoneGuitarTab.UI.Infrastructure
{
    using System.Collections.Generic;
    using PhoneGuitarTab.Data;

    public static class NavigationExtensions
    {
        public static void NavigateToTab(this INavigationService navService, Tab tab)
        {
            NavigateToTab(navService, new Dictionary<string, object>() {{"Tab", tab}});
        }
        public static void NavigateToTab(this INavigationService navService, Dictionary<string, object> parameters)
        {
            var tab = (Tab)parameters["Tab"];

            navService.NavigateTo(tab.TabType.Name == Strings.GuitarPro || tab.TabType.Name == Strings.MusicXml ? 
                Strings.StaveTab : 
                Strings.TextTab, parameters);
        }
    }
}
