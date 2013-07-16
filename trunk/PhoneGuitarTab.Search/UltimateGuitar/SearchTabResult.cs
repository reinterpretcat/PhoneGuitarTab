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
            "http://www.ultimate-guitar.com/search.php?band_name={0}&song_name={1}&type[]=200&type[]=400&type[]=500&type[]=600&version_la=&iphone=1&order=title_srt&page={2}&order_mode=ASC";

        private const string RequestTemplateGuitar =
            "http://www.ultimate-guitar.com/search.php?band_name={0}&song_name={1}&type[]=200&version_la=&iphone=1&order=title_srt&page={2}&order_mode=ASC";

        private const string RequestTemplateBass =
            "http://www.ultimate-guitar.com/search.php?band_name={0}&song_name={1}&type[]=400&version_la=&iphone=1&order=title_srt&page={2}&order_mode=ASC";

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

        public event DownloadStringCompletedEventHandler SearchComplete;

        private void InvokeSearchComplete(DownloadStringCompletedEventArgs e)
        {
            DownloadStringCompletedEventHandler handler = SearchComplete;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// Handler for WebClient that parses xml data into specific objects
        /// throws specific exceptions if error occurs
        /// </summary>
        public void Run(int pageNumber, TabulatureType type)
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += (s, e) =>
                                                  {
                                                      try
                                                      {
                                                          if (e.Error == null)
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
            client.DownloadStringAsync(new Uri(String.Format(GetRequestTemplate(type), _group, _song, pageNumber)));
        }

        private string GetRequestTemplate(TabulatureType tabType)
        {
            string requestTemplate;

            switch (tabType)
            {

                case TabulatureType.All:
                    requestTemplate = RequestTemplate;
                    break;
                case TabulatureType.Guitar:
                    requestTemplate = RequestTemplateGuitar;
                    break;
                case TabulatureType.Bass:
                    requestTemplate = RequestTemplateBass;
                    break;

                default:
                    requestTemplate = RequestTemplate;
                    break;
            }

            return requestTemplate;
        }
    }
}
