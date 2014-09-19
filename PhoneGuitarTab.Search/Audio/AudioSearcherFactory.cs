namespace PhoneGuitarTab.Search.Audio
{
    public interface IAudioSearcherFactory
    {
        IAudioSearcher CreateOnlineSearcher();
        IAudioSearcher CreateLocalSearcher();
    }

    public class AudioSearcherFactory : IAudioSearcherFactory
    {
        public IAudioSearcher CreateOnlineSearcher()
        {
            return new SoundCloudAudioSearcher();
        }

        public IAudioSearcher CreateLocalSearcher()
        {
            return new LocalAudioSearcher();
        }
    }
}