using System;

namespace PhoneGuitarTab.Core.Dependencies.Lifetime
{
    /// <summary>
    /// Wraps already created instance using WeakReference object
    /// </summary>
    internal class ExternalLifetimeManager : ILifetimeManager
    {
        private readonly WeakReference _reference;
        public ExternalLifetimeManager(object instance)
        {
            _reference = new WeakReference(instance);
        }

        public Type InterfaceType { get; set; }
        public Type TargetType { get; set; }
        public object[] CstorArgs { get; set; }
        

        /// <summary>
        /// returns instace if it exists
        /// </summary>
        public object GetInstance()
        {
            if (_reference.IsAlive)
                return _reference.Target;
            throw new ObjectDisposedException("instance");
        }

        public object GetInstance(string name)
        {
            return GetInstance();
        }

        public void Dispose()
        {
        }

        public System.Reflection.ConstructorInfo Constructor { get; set; }
    }
}
