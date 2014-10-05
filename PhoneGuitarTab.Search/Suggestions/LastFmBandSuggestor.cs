using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using PhoneGuitarTab.Search.Arts;
using PhoneGuitarTab.Search.Extensions;


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
                //Clear existing Results
                this.Results.Clear();
                var XMLroot = root.Element("artist");

                if (XMLroot != null)
                {
                    var similar = XMLroot.Element("similar");
                    //If Bands in collection are less than 10, than get suggestions for all the similar artists.
                    //Otherwise take only 3 similar artist for each of the root artist
                    if (BaseBands.Count < 10)
                    {
                        foreach (var artist in similar.Elements())
                        {
                            this.CreateEntry(artist);
                        }   
                    }
                    else //otherwise take only 3 similar artist
                    {
                        foreach (var artist in similar.Elements().Take(3))
                        {
                            this.CreateEntry(artist);
                        }   
                    }
                                      
                }
    
            }
            catch (Exception ex)
            {
                    
           
            }
            
        }

        private void CreateEntry(XElement elem)
        {
            var artistName = GetSafeValue(elem.Element("name"));
            if (!(this.BaseBands.Contains(artistName.TransLiterate(), StringComparer.OrdinalIgnoreCase) || this.SuggestedArtistsSoFar.Contains(artistName.TransLiterate(), StringComparer.OrdinalIgnoreCase)))
            {
                var Entry = new SearchMediaEntry();
                Entry.BandName = artistName;
                Entry.ExtraLargeImageUrl = GetImageUrl(elem, ImageSize.ExtraLarge);
                SuggestedArtistsSoFar.Add(artistName);
                Results.Add(Entry);
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
             
            this.SuggestedArtistsSoFar.Clear();
            //Run Suggestions for a maximum of 20 bands
            foreach (string band in bands.Take(20))
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
                        InvokeSearchComplete(e);                                        
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