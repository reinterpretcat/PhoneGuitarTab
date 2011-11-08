using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace PhoneGuitarTab.Search.Lastfm
{
    /// <summary>
    /// /http://www.lastfm.ru/api/show?service=407
    /// </summary>
    public class SearchImageResult
    {
        private const string RequestTemplate = "http://ws.audioscrobbler.com/2.0/?method=artist.getimages&artist={0}&api_key=b25b959554ed76058ac220b7b2e0a026&limit=10&order=popularity";
        public SearchImageResultSummary Summary { get; private set; }
        public List<SearchImageResultEntry> Entries { get; private set; }

        public string Artist;

        public event DownloadStringCompletedEventHandler SearchComplete;

        private void InvokeSearchComplete(DownloadStringCompletedEventArgs e)
        {
            DownloadStringCompletedEventHandler handler = SearchComplete;
            if (handler != null) handler(this, e);
        }

        public SearchImageResult(string artist)
        {
            Artist = artist;
            Entries = new List<SearchImageResultEntry>();
        }

        public void Run()
        {

            WebClient client = new WebClient();
            client.DownloadStringCompleted += (s, e) =>
                                                  {
                                                      try
                                                      {

                                                          if (e.Error == null)
                                                          // throw new BadCodeSearchException(HttpStatusCode.NotFound);
                                                          {

                                                              XDocument document = XDocument.Load(e.Result);
                                                              XElement root = document.Root;
                                                              if (root.Attribute("status").Value == "ok")
                                                              {
                                                                  XElement images = root.Element("images");
                                                                  Summary = SearchImageResultSummary.Create(images);
                                                                  foreach (XElement element in images.Descendants())
                                                                      Entries.Add(SearchImageResultEntry.Create(element));
                                                              }
                                                          }
                                                      }
                                                      finally
                                                      {
                                                          InvokeSearchComplete(e);
                                                      }
                                                  };
            client.DownloadStringAsync(new Uri(String.Format(RequestTemplate, Artist)));
        }
    }
}