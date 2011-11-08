using System;
using System.Net;


namespace PhoneGuitarTab.Search
{
    /// <summary>
    /// Represents a session
    /// </summary>
    public interface ISession
    {
        IAsyncResult BeginLogin(string login, string password, AsyncCallback callback, object state);
        void LogOut();

        bool IsAuthenticated { get; }
        IRequestInspector RequstInspector { get; }
    }
}
