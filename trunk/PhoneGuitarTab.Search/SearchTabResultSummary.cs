using System;
using System.Xml;

namespace PhoneGuitarTab.Search.UltimateGuitar
{
    public class SearchTabResultSummary
    {
        public int TotalResults { get; set; }
        public int PageCount { get; set; }
        public int TotalSongs { get; set; }
        public int TotalSongsFound { get; set; }
        public int BandsFound { get; set; }
    }
}
