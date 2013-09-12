using System;

namespace PhoneGuitarTab.Search.UltimateGuitar
{
    using System.Net;

    public class SearchTabDownloader: FileDownloader
    {
        private const string UrlTemplateFile = "http://ultimate-guitar.com/tab_download.php?tab_id={0}";

        private SearchTabResultEntry Entry { get; set; }

        public SearchTabDownloader(SearchTabResultEntry entry, string destination) : base(destination)
        {
            Entry = entry;
        }

        public override void Download()
        {
            string link;

            if (Entry.Type == "tab pro")
                link = String.Format(UrlTemplateFile, Entry.Id);
            else
                link = Entry.Url;

            base.Download(WebRequest.Create(link));
        }
    }
}
