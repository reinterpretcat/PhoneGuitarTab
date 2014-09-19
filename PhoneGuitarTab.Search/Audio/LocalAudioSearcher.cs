using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneGuitarTab.Search.Audio
{
    public class LocalAudioSearcher: IAudioSearcher
    {
        public void Run(string trackNameOrUrl)
        {
            throw new NotImplementedException();
        }

        public event AudioUrlRetrievedHandler SearchCompleted;

        public string AudioStreamEndPointUrl
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsAudioFound
        {
            get { throw new NotImplementedException(); }
        }
    }
}
