using System;
using System.Net;


namespace PhoneGuitarTab.Search.UltimateGuitar
{
    /// <summary>
    /// Decorator of the request to ultimate guitar resource
    /// </summary>
    public class UgHttpWebRequest : System.Net.WebRequest
    {
        private HttpWebRequest _request;
        public UgHttpWebRequest(string url)
        {
            _request = (HttpWebRequest)
                UgSession.Instance.RequstInspector.Inspect(WebRequest.Create(url));
        }

        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            return _request.BeginGetResponse(callback, state);
        }

        public override WebResponse EndGetResponse(IAsyncResult asyncResult)
        {
            return _request.EndGetResponse(asyncResult);
        }
    }
}
