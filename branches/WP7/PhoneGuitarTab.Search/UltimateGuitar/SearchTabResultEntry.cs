using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PhoneGuitarTab.Search.UltimateGuitar
{
    public class SearchTabResultEntry
    {
        /*<result name="harvest" artist="opeth" version="2" 
        url="http://app.ultimate-guitar.com/iphone/tab.php?id=133175" 
        rating="5" votes="16" type_2="" type="tab" id="133175"/>*/
        public string Id { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Version { get; set; }
        public string Url { get; set; }
        public string Rating { get; set; }
        public int Votes { get; set; }
        public string Type { get; set; }

        public static SearchTabResultEntry Create(XmlReader reader)
        {
            return new SearchTabResultEntry
                                             {
                                                 Id = reader["id"],
                                                 Name = reader["name"],
                                                 Artist = reader["artist"],
                                                 Version = reader["version"],
                                                 Url = reader["url"],
                                                 Rating = reader["rating"],
                                                 Votes = Int32.Parse(reader["votes"] ?? "0"),
                                                 Type = reader["type"]
                                             };
        }

    }
}
