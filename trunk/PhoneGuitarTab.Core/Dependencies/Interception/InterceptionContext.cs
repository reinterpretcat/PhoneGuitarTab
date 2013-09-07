using System;
using System.Collections.Generic;
using System.Linq;
using PhoneGuitarTab.Core.Dependencies.Interception.Proxies;

namespace PhoneGuitarTab.Core.Dependencies.Interception
{
    /// <summary>
    /// Represents interception context which provide the way create proxy objects and interact with interceptors
    /// </summary>
    internal static class InterceptionContext
    {
        static readonly List<IInterceptor> __interceptors = new List<IInterceptor>();
        static InterceptionContext()
        {
            __interceptors.Add(new InterfaceInterceptor());
        }

        /// <summary>
        /// Creates proxy from interface type and instance
        /// </summary>
        /// <param name="type">interface type</param>
        /// <param name="instance">instance of implementation type</param>
        public static IProxy CreateProxy(Type type, object instance)
        {
            var interceptor = GetInterceptor(type);
            return interceptor != null ? interceptor.CreateProxy(type, instance) : null;
        }

        /// <summary>
        /// Gets first interceptor which can intercept type
        /// </summary>
        public static IInterceptor GetInterceptor(Type type)
        {
            return __interceptors.SingleOrDefault(i => i.CanIntercept(type));
        }

        /// <summary>
        /// Gets component for type from interceptor
        /// </summary>
        public static Component GetComponent(Type type)
        {
            var interceptor = GetInterceptor(type);
            return interceptor != null ? interceptor.Resolve(type) : null;
        }

        /// <summary>
        /// Gets default interceptor
        /// </summary>
        public static IInterceptor GetInterceptor()
        {
            return __interceptors.First();
        }
    }
}
