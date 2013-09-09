using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using PhoneGuitarTab.Core.Cryptography;

namespace PhoneGuitarTab.Search.UltimateGuitar
{
    /// <summary>
    /// Represents session for requests to ultimate guitar resources
    /// </summary>
    public class UgSession : ISession, IDisposable
    {
        private string _userName;
        private readonly Uri _loginUri = new Uri("http://ultimate-guitar.com/forum/login.php");
        private string _hash;

        private static readonly object __monitor = new object();

        public CookieCollection Cookies { get; private set; }

        public bool IsAuthenticated { get; private set; }
        public IRequestInspector RequstInspector { get; private set; }

        private UgSession()
        {
            RequstInspector = new DefaultRequestInspector(); 
            //new UgRequestInspector();
        }

        private static UgSession _instance;
        public static UgSession Instance
        {
            get
            {
                lock (__monitor)
                {
                    //TODO or offline
                    if (_instance == null) 
                    {
                        _instance = new UgSession();
                    }
                }
                return _instance;
            }
        }


        public IAsyncResult BeginLogin(string login, string password, AsyncCallback callback, object state)
        {
            _userName = login;
            //calc MD5
            _hash = string.Concat(MD5Core.GetHash(password).Select(b => b.ToString("x2")).ToArray());

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_loginUri);
            request.AllowAutoRedirect = false;


            request.Method = "POST";
            CookieCollection cookies = new CookieCollection();
            cookies.Add(new Cookie("do", "login"));
            cookies.Add(new Cookie("vb_login_md5password", _hash));
            cookies.Add(new Cookie("vb_login_md5password_utf", _hash));
            cookies.Add(new Cookie("vb_login_username", _userName));
            cookies.Add(new Cookie("vb_login_password", ""));
            CookieContainer container = new CookieContainer();
            container.Add(_loginUri, cookies);
            request.CookieContainer = container;


            return request.BeginGetResponse(ar =>
                                                {
                                                    try
                                                    {
                                                        var response = (HttpWebResponse) request.EndGetResponse(ar);
                                                        //TODO validate response
                                                        IsAuthenticated = Validate(response);
                                                    }
                                                    finally
                                                    {
                                                        callback(ar);
                                                    }

                                                }, state);


        }
        /// <summary>
        /// Validates auth response
        /// </summary>
        /// <param name="response">Server response</param>
        /// <returns></returns>
        private bool Validate(HttpWebResponse response)
        {
            //just try to get session hash
            Cookies = response.Cookies;
            return response.Cookies["bbsessionhash"] != null || AuthExistInSetCookieHeader(response);
        }

        private bool AuthExistInSetCookieHeader(HttpWebResponse response)
        {
            //response.Cookies["bbsessionhash"] != null
            string setCookie = response.Headers["Set-Cookie"];
            return
                CanExtractCookie(setCookie, "bbuserid", response) &&
                CanExtractCookie(setCookie, "bbusername", response) &&
                CanExtractCookie(setCookie, "bbpassword", response) &&
                CanExtractCookie(setCookie, "bbsessionhash", response);
        }


        private bool CanExtractCookie(string setCookie, string name, HttpWebResponse response)
        {
            Regex regex = new Regex(String.Format(@"{0}=(?<value>\w+);", name));
            Match m = regex.Match(setCookie);
            if (!m.Success && String.IsNullOrEmpty(m.Groups["value"].Value))
                return false;
            Cookies.Add(new Cookie(name, m.Groups["value"].Value, "/", ".ultimate-guitar.com"));
            return true;
        }

        public void LogOut()
        {
            //TODO send request
            _instance = null;
        }

        public void Dispose()
        {
            LogOut();
        }
    }
}
