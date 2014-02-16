using System;

namespace PhoneGuitarTab.Core.Dependencies.Interception
{
    /// <summary>
    /// Represents a result of method invocation
    /// </summary>
    public interface IMethodReturn
    {
        /// <summary>
        /// Returns return value of method
        /// </summary>
        T GetReturnValue<T>();

        /// <summary>
        /// Exception which occured during method invocation
        /// </summary>
        Exception Exception { get; }
    }
}
