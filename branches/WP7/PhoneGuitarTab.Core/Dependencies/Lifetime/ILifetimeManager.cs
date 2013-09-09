using System;
using System.Reflection;

namespace PhoneGuitarTab.Core.Dependencies.Lifetime
{
    /// <summary>
    /// Manages lifetime of object creation
    /// </summary>
    public interface ILifetimeManager: IDisposable
    {
        /// <summary>
        /// Interface type
        /// </summary>
        Type InterfaceType { get; set; }

        /// <summary>
        /// Target type
        /// </summary>
        Type TargetType { get; set; }

        /// <summary>
        /// Constructor's signature
        /// </summary>
        ConstructorInfo Constructor { get; set; }

        /// <summary>
        /// Constructor's parameters
        /// </summary>
        object[] CstorArgs { get; set; }

        /// <summary>
        /// Returns instance of the target type
        /// </summary>
        object GetInstance();

        /// <summary>
        /// Returns instance of the target type using name provided
        /// </summary>
        object GetInstance(string name);
    }
}
