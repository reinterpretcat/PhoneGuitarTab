using System.Net;

namespace PhoneGuitarTab.Search
{
    public enum MediaSearchType
    {
        Artist,
        Track
    };
    public interface IMediaSearcher
    {
        void RunMediaSearch(string artist, string track);
        event DownloadStringCompletedEventHandler MediaSearchCompleted;
        MediaSearchType SearchType { get; set; }
        SearchMediaEntry Entry { get; }
    }
}
