using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace PhoneGuitarTab.Search.Lastfm
{
    public class LastFmSearch:IMediaSearcher
    {
      
        public event DownloadStringCompletedEventHandler MediaSearchCompleted;

        public enum ImageSize
        {
            Small,
            Large,
            ExtraLarge
        };

        public LastFmSearch()
        {
            this.Entry = new SearchMediaEntry();
        }

        public SearchMediaEntry Entry { get; private set; }
        private void InvokeSearchComplete(DownloadStringCompletedEventArgs e)
        {
            DownloadStringCompletedEventHandler handler = MediaSearchCompleted;
            if (handler != null) handler(this, e);
        }


        #region Fields

        public MediaSearchType SearchType{ get; set; }

        #endregion Fields

        #region Override methods

        protected void CreateEntries(XElement root, MediaSearchType searchType)
        {
            var XMLroot = (searchType == MediaSearchType.Artist) ? root.Element("artist") : root.Element("track");

            if (XMLroot != null)
            {
                Entry.Url = GetSafeValue(XMLroot.Element("url"));
                Entry.ImageUrl = GetImageUrl(XMLroot, ImageSize.Small);
                Entry.LargeImageUrl = GetImageUrl(XMLroot, ImageSize.Large);
                Entry.ExtraLargeImageUrl = GetImageUrl(XMLroot, ImageSize.ExtraLarge);

                if (searchType == MediaSearchType.Artist)
                {
                    Entry.BandName = GetSafeValue(XMLroot.Element("name"));
                    var bio = XMLroot.Element("bio");
                    Entry.Summary = StripAllEscapeSymbols(Unescape(StripTagsCharArray(bio.Element("summary").Value)));
                }
            }
        }

        protected string GetRequestTemplate(MediaSearchType type)
        {
            switch (type)
            {
                case MediaSearchType.Artist:
                    return
                        "http://ws.audioscrobbler.com/2.0/?method=artist.getinfo&artist={0}&autocorrect=1&api_key=dee2df7c96b013246bba7fe491be1f40";
                case MediaSearchType.Track:
                    return
                        "http://ws.audioscrobbler.com/2.0/?method=track.getinfo&track={1}&artist={0}&autocorrect=1&api_key=dee2df7c96b013246bba7fe491be1f40";
                default:
                    return "Search Type not provided";
            }
        }

        #endregion Override methods

        public void RunMediaSearch(string artist, string track)
        {
            SearchType = string.IsNullOrEmpty(track) ? MediaSearchType.Artist : MediaSearchType.Track;
           
            WebClient client = new WebClient();
            client.DownloadStringCompleted += (s, e) =>
            {
                try
                {
                    if (e.Error == null)
                    {
                        byte[] byteArray = Encoding.UTF8.GetBytes(e.Result);
                        MemoryStream stream = new MemoryStream(byteArray);
                        XDocument document = XDocument.Load(stream);
                        XElement root = document.Root;
                        if (root.Attribute("status").Value == "ok")
                        {
                            CreateEntries(root, SearchType);
                        }
                    }
                }
                catch (Exception)
                {
                    //xml parse exceptions. buried intentionally
                }
                finally
                {
                    InvokeSearchComplete(e);
                }
            };

            client.DownloadStringAsync(new Uri(String.Format(GetRequestTemplate(SearchType), artist, track)));
        }

        #region Helper methods

        private string GetSafeValue(XElement element)
        {
            return element != null ? element.Value : "";
        }

        private string GetImageUrl(XElement element, ImageSize imageSize)
        {
            if (SearchType == MediaSearchType.Track)
                element = element.Element("album");

            // small, medium, large, extralarge, mega..
            var image = element.Elements("image").ToList().FirstOrDefault(i =>
            {
                var xAttribute = i.Attribute("size");

                switch (imageSize)
                {
                    case ImageSize.Small:
                        return xAttribute != null && xAttribute.Value == "large";
                    case ImageSize.Large:
                        return xAttribute != null && xAttribute.Value == "medium";
                    case ImageSize.ExtraLarge:
                        return xAttribute != null && xAttribute.Value == "mega";
                    default:
                        return xAttribute != null && xAttribute.Value == "extralarge";
                }
            });


            return image != null ? image.Value : "";
        }

        /// <summary>
        ///     Remove HTML tags from string using char array.
        ///     There is other ways to do it, but it seems to be fastest one
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
        ///     Remove all escape special chars (looking like &x-x; from string using char array.
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
        ///     unescaping method tuned to fastest work with lastFm REST response
        /// </summary>
        private string Unescape(string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return inputString;

            string returnString = inputString;
            returnString = returnString.Replace("&apos;", "'");
            returnString = returnString.Replace("&quot;", "\"");
            returnString = returnString.Replace("&gt;", ">");
            returnString = returnString.Replace("&lt;", "<");
            returnString = returnString.Replace("&amp;", "and");
            returnString = returnString.Replace(String.Format("Read more about {0} on Last.fm.", Entry.BandName), string.Empty);

            return returnString;
        }

        #endregion Helper methods
    }
}