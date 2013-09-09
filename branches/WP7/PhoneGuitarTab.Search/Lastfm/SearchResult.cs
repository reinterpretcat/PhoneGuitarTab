using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace PhoneGuitarTab.Search.Lastfm
{
    /// <summary>
    /// lastfm API call result
    /// </summary>
    public abstract class SearchResult
    {
        
        public string Artist;

        public event DownloadStringCompletedEventHandler SearchCompleted;

        private void InvokeSearchComplete(DownloadStringCompletedEventArgs e)
        {
            DownloadStringCompletedEventHandler handler = SearchCompleted;
            if (handler != null) handler(this, e);
        }

        public SearchResult(string artist)
        {
            Artist = artist;
        }

        protected abstract void CreateEntries(XElement root);
        protected abstract string GetRequestTemplate();

        public void Run()
        {

            WebClient client = new WebClient();
            client.DownloadStringCompleted += (s, e) =>
                                                  {
                                                      try
                                                      {
                                                          if (e.Error == null)
                                                          {

                                                              byte[] byteArray = Encoding.UTF8.GetBytes(e.Result);
                                                              MemoryStream stream = new MemoryStream(byteArray);
                                                              XDocument document = XDocument.Load(stream);
                                                              XElement root = document.Root;
                                                              if (root.Attribute("status").Value == "ok")
                                                              {
                                                                  CreateEntries(root);
                                                              }
                                                          }
                                                      }
                                                      catch (Exception ex)
                                                      {
                                                          //xml parse exceptions. buried intentionally
                                                      }
                                                      finally
                                                      {
                                                          InvokeSearchComplete(e);
                                                      }
                                                  };
            client.DownloadStringAsync(new Uri(String.Format(GetRequestTemplate(), Artist)));
        }
    }
}