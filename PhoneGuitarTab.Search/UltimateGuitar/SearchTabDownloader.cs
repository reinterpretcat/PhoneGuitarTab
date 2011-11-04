using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Text;
using System.IO;
using System.Net;

namespace PhoneGuitarTab.Search.UltimateGuitar
{
    public class SearchTabDownloader: FileDownloader
    {
        private const string UrlTemplateFile = "http://ultimate-guitar.com/tab_download.php?tab_id={0}";

        private SearchTabResultEntry Entry { get; set; }


        public SearchTabDownloader(SearchTabResultEntry entry, string destination)
        {
            Entry = entry;
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            stream = store.OpenFile(destination, FileMode.CreateNew);
        }

        public override void Download()
        {
            string link;
           /* //only these types are supported by UrlTemplateFile link
            if ((Entry.Type != "guitar pro") && (Entry.Type != "power tab"))
            {
                //InvokeDownloadComplete(new EventArgs());
                //return;
                link = Entry.Url;
            }
            else
            {*/
            if (Entry.Type == "tab pro")
                link = String.Format(UrlTemplateFile, Entry.Id);
            else
                link = Entry.Url;
            //}

            base.Download(new UgHttpWebRequest(link));
        }
    }
}
