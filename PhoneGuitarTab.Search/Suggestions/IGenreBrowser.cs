using System.Collections.Generic;
using System.Net;
using PhoneGuitarTab.Search.Arts;

namespace PhoneGuitarTab.Search.Suggestions
{

    public interface IGenreBrowser
    {
        void Run(string genre);
        event DownloadStringCompletedEventHandler SuggestionSearchCompleted;
        List<SearchMediaEntry> Results { get; }
    }
}