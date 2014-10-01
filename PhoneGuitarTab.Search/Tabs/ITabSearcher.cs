using System.Collections.Generic;
using System.Net;
using PhoneGuitarTab.Search.Arts;

namespace PhoneGuitarTab.Search.Tabs
{
    public enum ResultsSortOrder
    {
        Alphabetical = 1,
        Popularity = 2
    }

    public interface ITabSearcher
    {
        void Run(string group, string song, int pageNumber, TabulatureType type, ResultsSortOrder sortBy);
        event DownloadStringCompletedEventHandler SearchComplete;
        SearchTabResultSummary Summary { get; }
        List<SearchTabResultEntry> Entries { get; }
    }
}