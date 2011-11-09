using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace PhoneGuitarTab.Search.Lastfm
{
    public class SearchInfoResult : SearchResult
    {
        public string Url { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }

        public SearchInfoResult(string artist)
            : base(artist)
        {
        }

        protected override void CreateEntries(XElement root)
        {
            Url = root.Element("artist").Element("url").Value;
            var bio = root.Element("artist").Element("bio");
            Summary = bio.Element("summary").Value;
            Content = bio.Element("content").Value;
        }

        protected override string GetRequestTemplate()
        {
            return "http://ws.audioscrobbler.com/2.0/?method=artist.getinfo&artist={0}&api_key=b25b959554ed76058ac220b7b2e0a026&limit=10&order=popularity";
        }
    }
}
