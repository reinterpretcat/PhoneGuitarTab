

namespace PhoneGuitarTab.Search
{
    public delegate void AudioUrlRetrievedHandler();

    public interface IAudioSearcher
    {
        void Run(string trackNameOrUrl);
        event AudioUrlRetrievedHandler SearchCompleted;
        string AudioStreamEndPointUrl { get; }
    }
}
