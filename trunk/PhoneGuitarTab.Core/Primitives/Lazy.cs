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
                if (!this.IsValueCreated)
                {
                    lock (this._sync)
                    {
                        if (!this.IsValueCreated)
                        {
                            this._instance = this._getter();
                            this.IsValueCreated = true;
                        }
                    }
                }
                return this._instance;
            }
        }

        public Lazy(Func<T> getter)
        {
            this._getter = getter;
        }
    }
}
