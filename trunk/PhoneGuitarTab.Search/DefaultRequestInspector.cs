using System.Net;

namespace PhoneGuitarTab.Search
{
    public class DefaultRequestInspector: IRequestInspector
    {
        public WebRequest Inspect(WebRequest request)
        {
            return request;
        }
    }
}
