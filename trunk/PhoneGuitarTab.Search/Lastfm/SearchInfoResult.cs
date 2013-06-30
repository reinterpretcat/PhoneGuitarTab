using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace PhoneGuitarTab.Search.Lastfm
{
    public class SearchInfoResult : SearchResult
    {
        #region Constructors

        public SearchInfoResult(string artist)
            : base(artist)
        {
        }

        #endregion Constructors


        #region Fields

        public string Url { get; set; }
        public string Summary { get; set; }
        public string BandName { get; set; }

        #endregion Fields


        #region Override methods

        protected override void CreateEntries(XElement root)
        {
            Url = root.Element("artist").Element("url").Value;
            BandName = root.Element("artist").Element("name").Value;
            var bio = root.Element("artist").Element("bio");
            Summary = StripAllEscapeSymbols(Unescape(StripTagsCharArray(bio.Element("summary").Value)));
        }

        protected override string GetRequestTemplate()
        {
            return "http://ws.audioscrobbler.com/2.0/?method=artist.getinfo&artist={0}&api_key=dee2df7c96b013246bba7fe491be1f40";
        }

        #endregion Override methods


        #region Helper methods

        /// <summary>
        /// Remove HTML tags from string using char array.
        /// There is other ways to do it, but it seems to be fastest one
        /// </summary>
        private string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        /// <summary>
        /// Remove all escape special chars (looking like &x-x; from string using char array.
        /// </summary>
        private string StripAllEscapeSymbols(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '&')
                {
                    inside = true;
                    continue;
                }
                if (let == ';')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        /// <summary>
        /// unescaping method tuned to fastest work with lastFm REST response
        /// </summary>
        private string Unescape(string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return inputString;
    
            string returnString = inputString;
            returnString = returnString.Replace("&apos;", "'");
            returnString = returnString.Replace("&quot;", "\"");
            returnString = returnString.Replace("&gt;", ">");
            returnString = returnString.Replace("&lt;", "<");
            returnString = returnString.Replace("&amp;", "&");
            returnString = returnString.Replace(String.Format("Read more about {0} on Last.fm.", BandName), string.Empty);

            return returnString;
        }

        #endregion Helper methods
    }
}
