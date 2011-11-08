using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

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
