using System;
using System.Net;

namespace PhoneGuitarTab.Search.UltimateGuitar
{
    /// <summary>
    /// attach auth info for ultimate guitar resources
    /// </summary>
    public class UgRequestInspector : IRequestInspector
    {
        public WebRequest Inspect(WebRequest request)
        {
            var httpRequest = request as HttpWebRequest;

            if (httpRequest == null)
                throw new ArgumentException("Invalid parameter: HttpWebRequest is expected", "request");

            if (!UgSession.Instance.IsAuthenticated)
                throw new InvalidOperationException(SR.AuthenticationRequired);

            //attach auth cookies
            var cookies = UgSession.Instance.Cookies.GetEnumerator();
            CookieContainer container = new CookieContainer();
            while (cookies.MoveNext())
                container.Add(new Uri(SR.UltimateGuitarBasePath),
                                                cookies.Current as Cookie);

            httpRequest.CookieContainer = container;
            return httpRequest;
        }


    }
}
