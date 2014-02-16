using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace PhoneGuitarTab.Search.UltimateGuitar
{
    public class UltimateGuitarTabSearcher
    {
        private const string RequestTemplateAll =
            "http://www.ultimate-guitar.com/search.php?band_name={0}&song_name={1}&type[]=200&type[]=300&type[]=400&type[]=500&type[]=700&type[]=800&version_la=&iphone=1&order=title_srt&page={2}&order_mode=ASC&tab_type_group=all";

        private const string RequestTemplateGuitarPro =
            "http://www.ultimate-guitar.com/search.php?band_name={0}&song_name={1}&type[]=500&version_la=&iphone=1&order=title_srt&page={2}&order_mode=ASC&tab_type_group=all";

        private const string RequestTemplateGuitar =
            "http://www.ultimate-guitar.com/search.php?band_name={0}&song_name={1}&type[]=200&version_la=&iphone=1&order=title_srt&page={2}&order_mode=ASC";

        private const string RequestTemplateBass =
            "http://www.ultimate-guitar.com/search.php?band_name={0}&song_name={1}&type[]=400&version_la=&iphone=1&order=title_srt&page={2}&order_mode=ASC";

        private const string RequestTemplateChords =
            "http://www.ultimate-guitar.com/search.php?band_name={0}&song_name={1}&type[]=300&version_la=&iphone=1&order=title_srt&page={2}&order_mode=ASC";

        private const string RequestTemplateDrum =
            "http://www.ultimate-guitar.com/search.php?band_name={0}&song_name={1}&type[]=700&version_la=&iphone=1&order=title_srt&page={2}&order_mode=ASC";

        private readonly Dictionary<string, string> _tabTypeMapping = new Dictionary<string, string>()
            { { "tab pro", "guitar pro" } };

        public SearchTabResultSummary Summary;
        public List<SearchTabResultEntry> Entries;

        private string _group;
        private string _song;

        public UltimateGuitarTabSearcher(string group, string song)
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
                                    if ((reader.Name == "results") && (reader.IsStartElement()))
                                        Summary = CreateResultSummary(reader);

                                    if ((reader.Name == "result") && (reader.IsStartElement()))
                                        Entries.Add(this.CreateResultEntry(reader));
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

        #region Factory methods

        private SearchTabResultEntry CreateResultEntry(XmlReader reader)
        {
            var type = reader["type"] ?? "";
            return new SearchTabResultEntry
            {
                Id = reader["id"],
                Name = reader["name"],
                Artist = reader["artist"],
                Version = reader["version"],
                Url = reader["url"],
                Rating = reader["rating"],
                Votes = Int32.Parse(reader["votes"] ?? "0"),
                Type = _tabTypeMapping.ContainsKey(type)? _tabTypeMapping[type]: type
            };
        }

        public SearchTabResultSummary CreateResultSummary(XmlReader reader)
        {
            return new SearchTabResultSummary
            {
                TotalResults = Int32.Parse(reader["count"] ?? "0"),
                PageCount = Int32.Parse(reader["pages"] ?? "0"),
                TotalSongs = Int32.Parse(reader["total"] ?? "0"),
                TotalSongsFound = Int32.Parse(reader["total_found"] ?? "0"),
                BandsFound = Int32.Parse(reader["bands_found"] ?? "0")
            };
        }

        #endregion

        private string GetRequestTemplate(TabulatureType tabType)
        {
            string requestTemplate;

            switch (tabType)
            {
                case TabulatureType.All:
                    requestTemplate = RequestTemplateAll;
                    break;
                case TabulatureType.Guitar:
                    requestTemplate = RequestTemplateGuitar;
                    break;
                case TabulatureType.Bass:
                    requestTemplate = RequestTemplateBass;
                    break;
                case TabulatureType.Chords:
                    requestTemplate = RequestTemplateChords;
                    break;
                case TabulatureType.Drum:
                    requestTemplate = RequestTemplateDrum;
                    break;
                case TabulatureType.GuitarPro:
                    requestTemplate = RequestTemplateGuitarPro;
                    break;
                default:
                    requestTemplate = RequestTemplateAll;
                    break;
            }

            return requestTemplate;
        }
    }
}
