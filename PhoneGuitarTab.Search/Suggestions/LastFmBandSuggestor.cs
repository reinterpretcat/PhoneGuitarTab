using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using PhoneGuitarTab.Search.Arts;


namespace PhoneGuitarTab.Search.Suggestions
{
    public class LastFmBandSuggestor: IBandSuggestor
    {
        public event DownloadStringCompletedEventHandler SuggestionSearchCompleted;

        public enum ImageSize
        {
            Small,
            Large,
            ExtraLarge
        };

        public LastFmBandSuggestor()
        {
            Results = new List<SearchMediaEntry>();
            SuggestedArtistsSoFar = new List<string>();
        }

        public List<SearchMediaEntry> Results { get; private set; }

        private List<string> BaseBands { get;  set; }
        private List<string> SuggestedArtistsSoFar { get; set; } 

        private void InvokeSearchComplete(DownloadStringCompletedEventArgs e)
        {
            DownloadStringCompletedEventHandler handler = SuggestionSearchCompleted;
            if (handler != null) handler(this, e);
        }

  

        #region Override methods

        protected void CreateEntries(XElement root)
        {
            try
            {
                var XMLroot = root.Element("artist");

                if (XMLroot != null)
                {
                    var similar = XMLroot.Element("similar");
                    foreach (var artist in similar.Elements())
                    {
                        var artistName = GetSafeValue(artist.Element("name"));
                        if (!(this.BaseBands.Contains(artistName) || this.SuggestedArtistsSoFar.Contains(artistName)))
                        {
                             var Entry = new SearchMediaEntry();
                            Entry.BandName = artistName;
                            Entry.ExtraLargeImageUrl = GetImageUrl(artist, ImageSize.ExtraLarge);
                            SuggestedArtistsSoFar.Add(artistName);
                            Results.Add(Entry);
                        }
                       
                    }
                }
    
            }
            catch (Exception ex)
            {
                    
           
            }
            
        }

        protected string GetRequestTemplate()
        {         
           var lang = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
           return "http://ws.audioscrobbler.com/2.0/?method=artist.getinfo&artist={0}&lang="+ lang + "&autocorrect=1&api_key=dee2df7c96b013246bba7fe491be1f40";
        }

        #endregion Override methods

        public void RunBandSuggestor(List<string> bands )
        {
            this.BaseBands = bands;
            foreach (string band in bands)
            {             
                  this.RunForSingleBand(band);
            }
           
            
        }

        private void RunForSingleBand(string artist)
        {
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
                            CreateEntries(root);
                        }
                    }
                }
                catch (Exception)
                {
                    //xml parse exceptions. buried intentionally
                }
                finally
                {
                    if (artist == BaseBands.LastOrDefault())
                    {
                                                                   
                        InvokeSearchComplete(e);
                    }
                     
                   
                }
            };

            client.DownloadStringAsync(new Uri(String.Format(GetRequestTemplate(), artist)));
        }

       
        #region Helper methods

        private string GetSafeValue(XElement element)
        {
            return element != null ? element.Value : "";
        }

        private string GetImageUrl(XElement element, ImageSize imageSize)
        {


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

      

        #endregion Helper methods
    }
}