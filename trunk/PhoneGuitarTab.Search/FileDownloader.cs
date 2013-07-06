using System;
using System.IO;
using System.Net;
using System.Windows;

namespace PhoneGuitarTab.Search
{
    /// <summary>
    /// Downloads files and stores it to destination
    /// </summary>
    public class FileDownloader: IDisposable
    {
        protected Stream stream;

        public delegate void DownloadEventHandler(object sender, DownloadCompletedEventArgs e);
        public event DownloadEventHandler DownloadComplete;


        #region Constructors

        public FileDownloader(Stream writeStream)
        {
            stream = writeStream;
        }

        protected FileDownloader()
        {
        }

        #endregion Constructors


        #region Public methods

        public virtual void Download()
        {
            //nothing to do
            //InvokeDownloadComplete(new DownloadCompletedEventArgs(true));
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
                try
                {
                    var response = request.EndGetResponse(r);

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
                    InvokeDownloadComplete(new DownloadCompletedEventArgs(false));
                }
                catch (BadCodeSearchException)
                {
                    throw;
                }
                catch (WebException e)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(
                    () =>
                    {
                        if (e.Status == WebExceptionStatus.RequestCanceled)
                            MessageBox.Show("Looks like your request was interrupted by tombstoning");
                        else
                        {
                            using (HttpWebResponse response = (HttpWebResponse)e.Response)
                            {
                                MessageBox.Show("I got an http error of: " + response.StatusCode.ToString());
                            }
                        }
                    });

                    InvokeDownloadComplete(new DownloadCompletedEventArgs(true));
                }
            }, null);
        }

        public void Dispose()
        {
            stream.Dispose();
        }

        #endregion Public methods


        #region Helper methods

        private void InvokeDownloadComplete(DownloadCompletedEventArgs e)
        {
            var handler = DownloadComplete;
            if (handler != null) handler(this, e);
        }

        #endregion Helper methods
    }
}
