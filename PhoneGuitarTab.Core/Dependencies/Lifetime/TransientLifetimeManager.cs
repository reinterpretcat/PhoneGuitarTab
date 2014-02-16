using System;
using System.Reflection;
using PhoneGuitarTab.Core.Dependencies.Interception;
using PhoneGuitarTab.Core.Dependencies.Interception.Proxies;

namespace PhoneGuitarTab.Core.Dependencies.Lifetime
{
    /// <summary>
    /// Every time build a new instance
    /// </summary>
    public class TransientLifetimeManager: ILifetimeManager
    {
        public Type InterfaceType { get; set; }
        public Type TargetType { get; set; }
        public object[] CstorArgs { get; set; }
        public System.Reflection.ConstructorInfo Constructor { get; set; }

        public TransientLifetimeManager()
        {
            
        }

        private ConstructorInfo _constructor;
        private ConstructorInfo ConstructorInstance
        {
            get
            {
                return _constructor = _constructor ?? (Constructor?? TypeHelper.GetConstructor(TargetType, CstorArgs) ); }
        }
 
        /// <summary>
        /// returns new instance of the target type
        /// </summary>
        /// <returns></returns>
        public object GetInstance()
        {
            var instance = ConstructorInstance.Invoke(CstorArgs);
            var proxy = InterceptionContext.CreateProxy(InterfaceType, instance);
            return proxy ?? instance;
        }

        /// <summary>
        /// returns new instance of the target type. The name parameters isn't used
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetInstance(string name)
        {
            return GetInstance();
        }

        public void Dispose()
        {
            //nothing to do
        }
    }
}
