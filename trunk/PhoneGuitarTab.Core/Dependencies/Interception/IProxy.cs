using System;
using System.Collections.Generic;
using PhoneGuitarTab.Core.Dependencies.Interception.Behaviors;

namespace PhoneGuitarTab.Core.Dependencies.Interception.Proxies
{
    /// <summary>
    /// Represents a behavior of proxy
    /// </summary>
    public interface IProxy
    {
        object Instance { get; set; }

        /// <summary>
        /// Adds new behavior to wrapped instance
        /// </summary>
        void AddBehavior(IBehavior behavior);

        /// <summary>
        /// Clear list of behaviors
        /// </summary>
        void ClearBehaviors();
    }
}
