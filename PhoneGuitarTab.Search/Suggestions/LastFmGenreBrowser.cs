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
    public class LastFmGenreBrowser: IGenreBrowser
    {
        public event DownloadStringCompletedEventHandler SuggestionSearchCompleted;
        public enum ImageSize
        {
            Small,
            Large,
            ExtraLarge
        };

        public LastFmGenreBrowser()
        {
            Results = new List<SearchMediaEntry>();
           
        }

        public List<SearchMediaEntry> Results { get; private set; }             

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
                var topArtists = root.Element("topartists");

                if (topArtists != null)
                {
                    foreach (var artist in topArtists.Elements())
                    {
                        this.CreateEntry(artist);
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
          
                var Entry = new SearchMediaEntry();
                Entry.BandName = artistName;
                Entry.ExtraLargeImageUrl = GetImageUrl(elem, ImageSize.ExtraLarge);               
                Results.Add(Entry);
        }

        protected string GetRequestTemplate()
        {         
           return "http://ws.audioscrobbler.com/2.0/?method=tag.gettopartists&tag={0}&api_key=dee2df7c96b013246bba7fe491be1f40";
        }

        #endregion Override methods

        public void Run(string genre )
        {                      
            this.Results.Clear();

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

            client.DownloadStringAsync(new Uri(String.Format(GetRequestTemplate(), genre)));
            
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