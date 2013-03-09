using System.Collections.Generic;
using System.Xml.Linq;

namespace PhoneGuitarTab.Search.Lastfm
{
    public class SearchImageResult : SearchResult
    {
        public List<SearchImageResultEntry> Entries { get; private set; }

        public SearchImageResult(string artist)
            : base(artist)
        {
        }

        protected override void CreateEntries(System.Xml.Linq.XElement root)
        {
            XElement images = root.Element("images");
            foreach (XElement element in images.Descendants())
                Entries.Add(SearchImageResultEntry.Create(element));
        }

        protected override string GetRequestTemplate()
        {
            return "http://ws.audioscrobbler.com/2.0/?method=artist.getimages&artist={0}&api_key=b25b959554ed76058ac220b7b2e0a026&limit=10&order=popularity";
        }
    }
}
