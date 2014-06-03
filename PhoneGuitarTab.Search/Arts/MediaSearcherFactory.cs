namespace PhoneGuitarTab.Search.Arts
{
    public interface IMediaSearcherFactory
    {
        IMediaSearcher Create();
    }

    public class MediaSearcherFactory : IMediaSearcherFactory
    {
        public IMediaSearcher Create()
        {
            return new LastFmMediaSearcher();
        }
    }
}