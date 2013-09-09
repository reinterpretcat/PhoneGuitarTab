using System;

namespace PhoneGuitarTab.Search
{
    public class DownloadCompletedEventArgs : EventArgs
    {
        public bool HadErrors { get; private set; }

        public DownloadCompletedEventArgs(bool hadErrors)
        {
            HadErrors = hadErrors;
        }
    }
}
