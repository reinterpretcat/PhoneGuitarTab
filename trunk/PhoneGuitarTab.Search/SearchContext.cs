using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;


namespace PhoneGuitarTab.Search
{
    /// <summary>
    /// Stores context for search operations. Singleton
    /// </summary>
    public class SearchContext:IDictionary<string, object>
    {
        private readonly Dictionary<string, object> _container = new Dictionary<string, object>();
        private SearchContext()
        {
        }

        #region IDictionary members

        public void Add(string key, object value)
        {
            _container.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _container.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _container.Keys; }
        }

        public bool Remove(string key)
        {
            return _container.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _container.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get { return _container.Values; }
        }

        public object this[string key]
        {
            get { return _container[key]; }
            set { _container[key] = value; }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            _container.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _container.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _container.ContainsKey(item.Key) && _container.ContainsValue(item.Value);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _container.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _container.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _container.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _container.GetEnumerator();
        }

        #endregion

    }
}
