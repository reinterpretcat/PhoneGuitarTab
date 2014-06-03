using System.Collections.Generic;
using System.Net;
using PhoneGuitarTab.Search.Arts;

namespace PhoneGuitarTab.Search.Tabs
{
    public interface ITabSearcher
    {
        void Run(string group, string song, int pageNumber, TabulatureType type);
        event DownloadStringCompletedEventHandler SearchComplete;
        SearchTabResultSummary Summary { get; }
        List<SearchTabResultEntry> Entries { get; }
    }
}