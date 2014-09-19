namespace PhoneGuitarTab.Search.Audio
{
    public delegate void AudioUrlRetrievedHandler(object sender);

    public interface IAudioSearcher
    {
        void Run(string trackNameOrUrl);
        event AudioUrlRetrievedHandler SearchCompleted;
        string AudioStreamEndPointUrl { get; }
        bool IsAudioFound { get; }
    }
}