using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using LazyListBox;

namespace PhoneGuitarTab.UI.Notation.Infrastructure
{
    public class VirtualizedTrackList : IList, INotifyCollectionChanged
    {
        private readonly Core.Tab.Track Track = new Core.Tab.Track();

        public VirtualizedTrackList(Core.Tab.Track track, bool useLazyLoading)
        {
            Track = track;
            this.useLazyLoading = useLazyLoading;
        }

        Dictionary<int, LazyTrackDataItem> cache = new Dictionary<int, LazyTrackDataItem>();
        bool useLazyLoading;

 
        public int Count
        {
            get { return Track.Measures.Count; }
        }

        public int IndexOf(object value)
        {
            if (value == null)
                return -1;

            return (value as LazyTrackDataItem).Index;
        }

        public object this[int index]
        {
            get
            {
                LazyTrackDataItem item;
                if (cache.TryGetValue(index, out item))
                    return item;

                item = new LazyTrackDataItem(index, Track.Measures[index]);
                item.IsLazy = useLazyLoading;
                if (!useLazyLoading)
                {
                    item.GoToState(LazyDataLoadState.Minimum);
                    item.GoToState(LazyDataLoadState.Loading);
                }
                cache[index] = item;

                return item;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Insert(int newIndex, object value)
        {
            int[] sortedKeys = cache.Keys.ToArray();
            Array.Sort(sortedKeys);

            for (int i = sortedKeys.Length - 1; i >= 0; i--)
            {
                int key = sortedKeys[i];
                if (key < newIndex)
                    break;

                cache[key + 1] = cache[key];
                cache[key].Index++;
            }

            cache[newIndex] = (LazyTrackDataItem)value;
            var handler = CollectionChanged;
            if (handler != null)
                handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, newIndex));
        }

        #region E_NOT_IMPL

        #region IList Members

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion
    }
}
