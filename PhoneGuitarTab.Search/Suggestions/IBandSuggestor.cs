using System.Collections.Generic;
using System.Net;
using PhoneGuitarTab.Search.Arts;

namespace PhoneGuitarTab.Search.Suggestions
{

    public interface IBandSuggestor
    {
        void RunBandSuggestor(List<string> BaseBands);
        event DownloadStringCompletedEventHandler SuggestionSearchCompleted;
        List<SearchMediaEntry> Results { get; }
    }
}