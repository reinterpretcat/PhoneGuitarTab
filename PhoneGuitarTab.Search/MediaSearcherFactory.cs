using PhoneGuitarTab.Search.Lastfm;

namespace PhoneGuitarTab.Search
{
    public class MediaSearcherFactory:IMediaSearcherFactory
    {
        public IMediaSearcher Create()
        {
            return new LastFmSearch();
        }
    }
}
