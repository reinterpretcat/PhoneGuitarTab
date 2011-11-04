using System;
using System.Net;


namespace PhoneGuitarTab.Search
{
    /// <summary>
    /// Represents a session
    /// </summary>
    public interface ISession
    {
        //void LogIn(string login, string password);
        void LogOut();

        bool IsAuthenticated { get; }
        IRequestInspector RequstInspector { get; }
    }
}
