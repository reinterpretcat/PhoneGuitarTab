using System;
using System.Collections.Generic;
using System.Linq;

namespace PhoneGuitarTab.Core.Navigation
{
    public class PageMapping
    {
        private Dictionary<IPageType, Tuple<Uri, Lazy<ViewModel>>> _pageMapping;

        public PageMapping(Dictionary<IPageType, Tuple<Uri, Lazy<ViewModel>>> mapping)
        {
            _pageMapping = mapping;
        }

        public Uri GetUri(IPageType page)
        {
            return _pageMapping.Single(m => m.Key.Equals(page)).Value.Item1;
        }

        public ViewModel GetViewModel(IPageType page)
        {
            return _pageMapping.Single(m => m.Key.Equals(page)).Value.Item2.Value;
        }

        public IPageType GetPageType(Uri pageUri)
        {
            return _pageMapping.Keys.Single(m => _pageMapping[m].Item1 == pageUri);
        }

    }
}
