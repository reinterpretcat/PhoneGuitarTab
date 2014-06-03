using System;
using System.Net;

namespace PhoneGuitarTab.Search.Audio
{
    public class SoundCloudAudioSearcher:IAudioSearcher 
    {
        
        //The concrete stream url of the soundcloud endpoint for the track.
        //This URL is used in audio player.
        public string AudioStreamEndPointUrl { get; private set; }

        public event AudioUrlRetrievedHandler SearchCompleted;

        public void Run(string trackUrl)
        {

            if (!string.IsNullOrEmpty(trackUrl))
            {
                var request = (HttpWebRequest)WebRequest
                    .Create(trackUrl + ".json?client_id=5ca9c93662aaa8d953a421ce53500bae");
                request.Method = "HEAD";
                request.AllowReadStreamBuffering = true;
                request.AllowAutoRedirect = true;
                request.BeginGetResponse(ReadWebRequestCallback, request);
            }
        }

        private void ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            var myRequest = (HttpWebRequest) callbackResult.AsyncState;
            var myResponse = (HttpWebResponse) myRequest.EndGetResponse(callbackResult);
            AudioStreamEndPointUrl = myResponse.ResponseUri.AbsoluteUri;
            
            SearchCompleted();
            myResponse.Close();
        }
    }
}