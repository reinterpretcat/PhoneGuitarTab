using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PhoneGuitarTab.Search
{
    public class FileDownloader
    {
         public event EventHandler DownloadComplete;

        private void InvokeDownloadComplete(EventArgs e)
        {
            EventHandler handler = DownloadComplete;
            if (handler != null) handler(this, e);
        }

        public void Download(string url, string destination)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.BeginGetResponse(r =>
            {
                var response = request.EndGetResponse(r);
                try
                {

                    // Open the response stream
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
                        using (var fileStream = store.OpenFile(destination, FileMode.CreateNew))
                        {
                            // Create a 4K buffer to chunk the file
                            byte[] myBuffer = new byte[4096];
                            int bytesRead;
                            while (0 <
                                   (bytesRead =
                                    responseStream.Read(myBuffer, 0, myBuffer.Length)))
                                fileStream.Write(myBuffer, 0, bytesRead);
                        }

                    }

                }
                catch (BadCodeSearchException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new SearchExceptions(SR.SearchResultDownloadUnexpected, ex);
                }
                finally
                {
                    InvokeDownloadComplete(new EventArgs());
                }

            }, null);
        }
    }
}
