using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneGuitarTab.Search.SoundCloud
{
    using System.Net;
    using System.IO;

    public class SoundCloudSearch
    {
       
        private string TrackUrl { get; set; }

        //The concrete stream url of the soundcloud endpoint for the track.
        //This URL is used in audio player.
        public string AudioStreamEndPointUrl { get; private set; }

        public event AudioUrlRetrievedHandler SearchCompleted;
        public delegate void AudioUrlRetrievedHandler(object sender);

        public SoundCloudSearch(string trackUrl)
        {
            this.TrackUrl = trackUrl;
        }

        public void Run()
        {
            if (!string.IsNullOrEmpty(TrackUrl))
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(TrackUrl + ".json?client_id=5ca9c93662aaa8d953a421ce53500bae");
                request.Method = "HEAD";
                request.AllowReadStreamBuffering = true;
                request.AllowAutoRedirect = true;
                request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallback), request);
            }
        }

        private void ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult);


            using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
            {
                this.AudioStreamEndPointUrl = myResponse.ResponseUri.AbsoluteUri;
                this.SearchCompleted(this);
            }
            myResponse.Close();

        }

    }
}
