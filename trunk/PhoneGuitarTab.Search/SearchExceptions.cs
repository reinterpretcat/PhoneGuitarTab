using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace PhoneGuitarTab.Search
{
    public class SearchExceptions : Exception
    {
        public SearchExceptions() : base() { }
        public SearchExceptions(string s) : base(s) { }
        public SearchExceptions(string s, Exception e) : base(s, e) { }
    }

    public class BadCodeSearchException : Exception
    {
        public HttpStatusCode StatusCode;
        public BadCodeSearchException(HttpStatusCode code)
            : base()
        {
            StatusCode = code;
        }
    }

    public class UnexpectedFormatSearchException : Exception
    {
        public string Node;
        public UnexpectedFormatSearchException(string node)
            : base()
        {
            Node = node;
        }
    }

}
