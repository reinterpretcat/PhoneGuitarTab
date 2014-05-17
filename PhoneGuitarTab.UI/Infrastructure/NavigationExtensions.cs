using System.Collections.Generic;
using PhoneGuitarTab.Core.Services;
using PhoneGuitarTab.UI.Data;

namespace PhoneGuitarTab.UI.Infrastructure
{
    public static class NavigationExtensions
    {
        public static void NavigateToTab(this INavigationService navService, Tab tab)
        {
            NavigateToTab(navService, new Dictionary<string, object> {{"Tab", tab}});
        }

        public static void NavigateToTab(this INavigationService navService, Dictionary<string, object> parameters)
        {
            var tab = (Tab) parameters["Tab"];

            navService.NavigateTo(tab.TabType.Name == TabTypeNames.GuitarPro || tab.TabType.Name == TabTypeNames.MusicXml
                ? NavigationViewNames.StaveTab
                : NavigationViewNames.TextTab, parameters);
        }
    }
}