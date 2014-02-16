using System;
using System.Collections.Generic;
using PhoneGuitarTab.Core.Dependencies.Interception.Behaviors;
using PhoneGuitarTab.Core.Dependencies.Interception.Proxies;

namespace PhoneGuitarTab.Core.Dependencies.Interception
{
    public class InterfaceInterceptor: IInterceptor
    {
        protected readonly Dictionary<Type, Component> ProxyComponentMapping = new Dictionary<Type, Component>();
        protected readonly List<IBehavior> Behaviors = new List<IBehavior>();
        
        /// <summary>
        /// True, if type can be intercepted
        /// </summary>
        /// <param name="type">interface type</param>
        public bool CanIntercept(Type type)
        {
            if (type == null)
                return false;
            return ProxyComponentMapping.ContainsKey(type) && ProxyComponentMapping[type].CanCreateProxy;
        }

        public Component Resolve(Type type)
        {
            //Resolve from mapping
            return ProxyComponentMapping[type];
        }

        /// <summary>
        /// Return proxy for the type
        /// </summary>
        public IProxy CreateProxy(Type type, object instance)
        {
            var component = Resolve(type);
            var proxy = component.CreateProxy(instance, Behaviors);
            return proxy;
        }

        public void Register(Type type, Component component)
        {
            ProxyComponentMapping.Add(type, component);
        }
    }
}
