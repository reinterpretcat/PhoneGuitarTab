using System;

namespace PhoneGuitarTab.Search
{
    public class DownloadCompletedEventArgs : EventArgs
    {
        public bool HasErrors { get; private set; }

        public DownloadCompletedEventArgs(bool hasErrors)
        {
            HasErrors = hasErrors;
        }
    }
}