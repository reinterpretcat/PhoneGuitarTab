using System;

namespace PhoneGuitarTab.Core.Primitives
{
    public class Lazy<T>
    {
        private readonly object _sync = new object();
        private readonly Func<T> _getter;
        private T _instance;

        public bool IsValueCreated { get; private set; }

        public T Value
        {
            get
            {
                if (!IsValueCreated)
                {
                    lock (_sync)
                    {
                        if (!IsValueCreated)
                        {
                            _instance = _getter();
                            IsValueCreated = true;
                        }
                    }
                }
                return _instance;
            }
        }

        public Lazy(Func<T> getter)
        {
            _getter = getter;
        }
    }
}