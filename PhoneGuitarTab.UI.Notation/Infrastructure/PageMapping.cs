using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using PhoneGuitarTab.UI.Notation.Persistence;
using PhoneGuitarTab.UI.Notation.ViewModel;

namespace PhoneGuitarTab.UI.Notation.Infrastructure
{
    public class PageMapping<T> where T : ViewModelBase
    {
        private static Dictionary<PageType, KeyValuePair<Uri, Lazy<T>>> pageMapping = null;

        public static void InitMapping(Dictionary<PageType, KeyValuePair<Uri, Lazy<T>>> mapping)
        {
            pageMapping = mapping;
        }

        public static PageType GetPageTypeByTab(Tab tab)
        {
            switch (tab.Type)
            {
                case "guitar pro":
                    return PageType.Tab;
                default:
                    return  PageType.TextTab;
            }
        }

        public static Uri GetUri(PageType pageType){
            return pageMapping.Single(m => m.Key == pageType).Value.Key;
        }

        public static T GetViewModel(Uri uri){
            return pageMapping.Values.Single(kvp => kvp.Key == uri).Value.Value;
        }

        public static T GetViewModel(PageType pageType){
            return pageMapping.Single(m => m.Key == pageType).Value.Value.Value;
        }
    }
}
