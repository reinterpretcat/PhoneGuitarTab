
namespace PhoneGuitarTab.UI.Infrastructure
{
    using System.Collections.Generic;
    using PhoneGuitarTab.Core.Navigation;
    using PhoneGuitarTab.Data;

    public static class NavigationExtensions
    {
        public static void NavigateToTab(this INavigationService navService, Tab tab)
        {
            NavigateToTab(navService, new Dictionary<string, object>() {{"Tab", tab}});
        }
        public static void NavigateToTab(this INavigationService navService, Dictionary<string, object> parameters)
        {
            Tab tab = (Tab)parameters["Tab"];

            if(tab.TabType.Name == Strings.GuitarPro)
            {
                navService.NavigateTo(Strings.StaveTab, parameters);
            }
            else
            {
                navService.NavigateTo(Strings.TextTab, parameters);
            }
            
        }
    }
}
