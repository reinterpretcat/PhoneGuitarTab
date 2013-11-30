using System;

namespace PhoneGuitarTab.UI.Infrastructure
{
    /// <summary>
    /// Provides the way to subscribe and/or publish events globally
    /// Not sure that it's the best approach from architecture point of view
    /// </summary>
    public sealed class MessageHub
    {
        public event EventHandler<int> CollectionTabRemoved;
        public void RaiseCollectionTabRemoved(int id)
        {
            var handler = CollectionTabRemoved;
            if (handler != null)
                handler(this, id);
        }

        public event EventHandler<int> HistoryTabRemoved;
        public void RaiseHistoryTabRemoved(int id)
        {
            var handler = HistoryTabRemoved;
            if (handler != null)
                handler(this, id);
        }

        public event EventHandler<int> GroupTabRemoved;
        public void RaiseGroupTabRemoved(int id)
        {
            var handler = GroupTabRemoved;
            if (handler != null)
                handler(this, id);
        }

        public event EventHandler TabsDownloaded;
        public void RaiseTabsDownloaded()
        {
            var handler = TabsDownloaded;
            if (handler != null)
                handler(this, new EventArgs());
        }

        public event EventHandler TabsRefreshed;
        public void RaiseTabsRefreshed()
        {
            var handler = TabsRefreshed;
            if (handler != null)
                handler(this, new EventArgs());
        }
    }
}
