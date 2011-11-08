using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;


namespace PhoneGuitarTab.Search
{
    /// <summary>
    /// Downloads files and stores it to destination
    /// </summary>
    public class FileDownloader: IDisposable
    {
        protected Stream stream; 
        public event EventHandler DownloadComplete;


        public FileDownloader(Stream writeStream)
        {
            stream = writeStream;
        }

        protected FileDownloader()
        {
        }

        private void InvokeDownloadComplete(EventArgs e)
        {
            EventHandler handler = DownloadComplete;
            if (handler != null) handler(this, e);
        }

        public virtual void Download()
        {
            //nothing to do
            InvokeDownloadComplete(new EventArgs());
        }

        /// <summary>
        /// Download via HttpWebRequest
        /// </summary>
        /// <param name="url"></param>
        public void Download(string url)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            Download(request);
        }

        /// <summary>
        /// Download via decorated WebRequest
        /// </summary>
        /// <typeparam name="T">type of WebRequest</typeparam>
        /// <param name="request">decorated WebRequest</param>
        public void Download<T>(T request) where T: WebRequest
        {
            request.BeginGetResponse(r =>
            {
                var response = request.EndGetResponse(r);
                try
                {
                    // Open the response stream
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        //TODO: catch error here
                        // Create a 4K buffer to chunk the file
                        byte[] myBuffer = new byte[4096];
                        int bytesRead;
                        while (0 <
                               (bytesRead =
                                responseStream.Read(myBuffer, 0, myBuffer.Length)))
                            stream.Write(myBuffer, 0, bytesRead);
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

        public void Dispose()
        {
            stream.Dispose();
        }
    }
}
