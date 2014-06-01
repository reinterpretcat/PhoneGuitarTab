using System;
using System.IO;
using System.Net;

namespace PhoneGuitarTab.Search.SoundCloud
{
    public class SoundCloudSearch:IAudioSearcher 
    {
        
        //The concrete stream url of the soundcloud endpoint for the track.
        //This URL is used in audio player.
        public string AudioStreamEndPointUrl { get; private set; }

        public event AudioUrlRetrievedHandler SearchCompleted;

        public void Run(string trackUrl)
        {

            if (!string.IsNullOrEmpty(trackUrl))
            {
                HttpWebRequest request =
                    (HttpWebRequest)WebRequest.Create(trackUrl + ".json?client_id=5ca9c93662aaa8d953a421ce53500bae");
                request.Method = "HEAD";
                request.AllowReadStreamBuffering = true;
                request.AllowAutoRedirect = true;
                request.BeginGetResponse(ReadWebRequestCallback, request);
            }
        }

        private void ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest myRequest = (HttpWebRequest) callbackResult.AsyncState;
            HttpWebResponse myResponse = (HttpWebResponse) myRequest.EndGetResponse(callbackResult);
            AudioStreamEndPointUrl = myResponse.ResponseUri.AbsoluteUri;
            
            SearchCompleted();
            myResponse.Close();
        }
    }
}