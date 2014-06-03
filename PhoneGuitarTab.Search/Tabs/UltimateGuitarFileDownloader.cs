using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using PhoneGuitarTab.Search.Arts;
using PhoneGuitarTab.Tablatures.Readers;
using PhoneGuitarTab.Tablatures.Writers;
using SharpGIS;

namespace PhoneGuitarTab.Search.Tabs
{
    public class UltimateGuitarFileDownloader : FileDownloader
    {
        // This url points to "dirty" gp file. don't know how to restore original one.
        //private const string UrlTemplateFile = "http://ultimate-guitar.com/tab_download.php?tab_id={0}";

        private const string GuitarProDetailsUrlTemplate =
            "http://app.ultimate-guitar.com/iphone/tab.php?app_platform=&id={0}";

        private SearchTabResultEntry Entry { get; set; }

        public UltimateGuitarFileDownloader(SearchTabResultEntry entry, string destination) : base(destination)
        {
            Entry = entry;
        }

        private Task<string> GetGuitarProDownloadUrl(string id)
        {
            var request = WebRequest.Create(string.Format(GuitarProDetailsUrlTemplate, id));
            return
                Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null)
                    .ContinueWith(t =>
                    {
                        // TODO handle errors
                        try
                        {
                            var response = t.Result;
                            using (var responseStream = response.GetResponseStream())
                            {
                                var xDoc = XDocument.Load(responseStream);
                                return xDoc.Root.Attribute("gp_url").Value;
                            }
                        }
                        catch (AggregateException)
                        {
                            return String.Empty;
                        }
                    });
        }

        public override void Download()
        {
            // NOTE ultimate guitar service prohibits unauthorized users to download raw gp files
            // By now, we are do not force users to create any accounts as there is another way to get gp-files
            // unfortunately, these files are json-serialized representation of gp, so we need to perform additional manipulations 
            if (Entry.Type == "guitar pro")
            {
                GetGuitarProDownloadUrl(Entry.Id).ContinueWith(task =>
                {
                    var link = task.Result;
                    if (link != string.Empty)
                    {
                        GZipWebClient client = new GZipWebClient();

                        client.DownloadStringAsync(new Uri(link));
                        client.DownloadStringCompleted += (o, e) =>
                        {
                            // convert from json to guitar pro format
                            var songReader = new JsonSongReader(e.Result);
                            var song = songReader.ReadSong();

                            var songWriter = new FifthGuitarProSongWriter(new BinaryWriter(GetOutputStream()));
                            songWriter.WriteSong(song);

                            InvokeDownloadComplete(new DownloadCompletedEventArgs(false));
                        };
                    }
                });
            }
            else
            {
                Download(WebRequest.Create(Entry.Url));
            }
        }
    }
}