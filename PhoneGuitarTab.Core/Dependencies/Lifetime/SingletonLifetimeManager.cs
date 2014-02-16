using System;
using PhoneGuitarTab.Core.Dependencies.Interception;
using PhoneGuitarTab.Core.Dependencies.Interception.Proxies;

namespace PhoneGuitarTab.Core.Dependencies.Lifetime
{
    /// <summary>
    /// Creates singleton instance for wrapped type
    /// </summary>
    public class SingletonLifetimeManager : ILifetimeManager
    {
        public Type InterfaceType { get; set; }
        public Type TargetType { get; set; }
        public object[] CstorArgs { get; set; }
        public System.Reflection.ConstructorInfo Constructor { get; set; }

        public SingletonLifetimeManager()
        {
            
        }

        private object _instance;
        private IProxy _proxy;

        /// <summary>
        /// Returns singleton instance
        /// </summary>
        /// <returns></returns>
        public object GetInstance()
        {
            return GetInstance(String.Empty);
        }

        /// <summary>
        /// returns new instance of the target type. The name parameters isn't used
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetInstance(string name)
        {
            if (_instance == null)
            {
                _instance = (Constructor ?? TypeHelper.GetConstructor(TargetType, CstorArgs))
                    .Invoke(CstorArgs);
                _proxy = InterceptionContext.CreateProxy(InterfaceType, _instance);
            }
            return _proxy ?? _instance;
        }

        public void Dispose()
        {
            if (_instance is IDisposable)
                (_instance as IDisposable).Dispose();
            _instance = null;
        }

        
    }
}
