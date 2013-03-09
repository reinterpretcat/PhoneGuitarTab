using System.Net;

namespace PhoneGuitarTab.Search
{
    /// <summary>
    /// Modifies requests
    /// </summary>
    public interface IRequestInspector
    {
        WebRequest Inspect(WebRequest request);
    }
}
