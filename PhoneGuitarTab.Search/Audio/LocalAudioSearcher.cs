using System;

using Windows.Storage;
using Windows.Storage.FileProperties;
using Microsoft.Xna.Framework.Media;

namespace PhoneGuitarTab.Search.Audio
{
    //TODO - This class is to be implemented when WP 8.0 support is finished.
    //WP 8.0 does not allow getting the concrete URI's of the local songs but 8.1 does.
    public class LocalAudioSearcher: IAudioSearcher
    {
        public void Run(string trackNameOrUrl)
        {
           // Environment.OSVersion.
            //using (MediaLibrary library = new MediaLibrary())
            //{
            //    foreach (var item in library.Songs)
            //    {
            //        if (item.Name == trackNameOrUrl)
            //        {
                    
            //         //   AudioStreamEndPointUrl = new Uri(item, UriKind.Relative);

            //        }
            //    }
            //} 
        }

        public event AudioUrlRetrievedHandler SearchCompleted;

        public string AudioStreamEndPointUrl
        {
            get;
            private set;
        }

        public bool IsAudioFound { get; private set; }
    }
}
