using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PhoneGuitarTab.Search.UltimateGuitar
{
    public class SearchTabResultSummary
    {
        //<results count="292" page="1" pages="2" total="71" total_found="71" one_band="" order="myweight" bands_found="1">
        public int TotalResults { get; set; }
        public int PageCount { get; set; }
        public int TotalSongs { get; set; }
        public int TotalSongsFound { get; set; }
        public int BandsFound { get; set; }

        public static SearchTabResultSummary Create(XmlReader reader)
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
    }
}
