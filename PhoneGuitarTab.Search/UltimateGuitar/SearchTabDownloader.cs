using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Text;
using System.IO;
using System.Net;

namespace PhoneGuitarTab.Search.UltimateGuitar
{
    public class SearchTabDownloader:FileDownloader
    {
        private const string UrlTemplateFile = "http://ultimate-guitar.com/tab_download.php?tab_id={0}";

        private SearchTabResultEntry Entry { get; set; }


        public SearchTabDownloader(SearchTabResultEntry entry)
        {
            Entry = entry;
        }
        
        public void Download(string destination)
        {
            string link;
            //only these types are supported by UrlTemplateFile link
            if ((Entry.Type != "guitar pro") && (Entry.Type != "power tab"))
            {
                //InvokeDownloadComplete(new EventArgs());
                //return;
                link = Entry.Url;
            }
            else
            {
                link = String.Format(UrlTemplateFile, Entry.Id);
            }

           Download(link,destination);

        }
    }
}
