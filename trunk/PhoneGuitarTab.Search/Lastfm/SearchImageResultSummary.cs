using System;
using System.Xml.Linq;

namespace PhoneGuitarTab.Search.Lastfm
{
    public class SearchImageResultSummary
    {
        public string Artist { get; set; }
        public int CurrentPage { get; set; }
        public int PerPage { get; set; }
        public int TotalPages { get; set; }
        public int Total { get; set; }

        public static SearchImageResultSummary Create(XElement node)
        {
            return new SearchImageResultSummary
                                                   {
                                                       Artist = node.Attribute("artist").Value,
                                                       CurrentPage = Int32.Parse(node.Attribute("page").Value),
                                                       PerPage = Int32.Parse(node.Attribute("perPage").Value),
                                                       TotalPages = Int32.Parse(node.Attribute("totalPages").Value),
                                                       Total = Int32.Parse(node.Attribute("total").Value)
                                                   };
        }
    }
}
