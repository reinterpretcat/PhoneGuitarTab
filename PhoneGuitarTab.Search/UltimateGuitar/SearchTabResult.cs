using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace PhoneGuitarTab.Search.UltimateGuitar
{
    public class SearchTabResult
    {
        private const string RequestTemplate =
            "http://www.ultimate-guitar.com/search.php?band_name={0}&song_name={1}&type[]=500&version_la=&iphone=1&order=band_name_srt&page={2}";

        public SearchTabResultSummary Summary;
        public List<SearchTabResultEntry> Entries;

        private string _group;
        private string _song;

        public SearchTabResult(string group, string song)
        {
            this._group = group;
            this._song = song;
            Summary = new SearchTabResultSummary();
            Entries = new List<SearchTabResultEntry>();
        }

       /* public Uri RequestUri
        {
            get { return new Uri(String.Format(RequestTemplate, _group, _song)); }
        }*/

        public event DownloadStringCompletedEventHandler SearchComplete;

        private void InvokeSearchComplete(DownloadStringCompletedEventArgs e)
        {
            DownloadStringCompletedEventHandler handler = SearchComplete;
            if (handler != null) handler(this, e);
        }

        /*public IEnumerable<string> PageLinks
        {
           get
           {
               for (int i = 1; i <= Summary.PageCount; i++)
                   yield return String.Format(RequestTemplate,
                       _group, _song, i);
           }
        }*/

        /// <summary>
        /// Handler for WebClient that parses xml data into specific objects
        /// throws specific exceptions if error occurs
        /// </summary>
        public void Run(int pageNumber)
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += (s, e) =>
                                                  {
                                                      try
                                                      {

                                                          if (e.Error == null)
                                                             // throw new BadCodeSearchException(HttpStatusCode.NotFound);

                                                          {
                                                              using (XmlReader reader = XmlReader.Create(new StringReader(e.Result)))
                                                              {
                                                                  while (reader.Read())
                                                                  {
                                                                      if ((reader.Name == "results") &&
                                                                          (reader.IsStartElement()))
                                                                          Summary = SearchTabResultSummary.Create(reader);

                                                                      if ((reader.Name == "result") &&
                                                                          (reader.IsStartElement()))
                                                                          Entries.Add(SearchTabResultEntry.Create(reader));
                                                                  }
                                                              }

                                                          }
                                                      }
                                                      catch/* (Exception ex)*/
                                                      {
                                                          /*throw new SearchExceptions(SR.SearchResultRunUnexpected,
                                                                                     ex);*/
                                                      }
                                                      finally
                                                      {
                                                          InvokeSearchComplete(e);
                                                      }
                                                  };
            client.DownloadStringAsync(new Uri(String.Format(RequestTemplate, _group, _song, pageNumber)));
        }
    }
}
