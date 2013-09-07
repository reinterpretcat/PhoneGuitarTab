using System;

namespace PhoneGuitarTab.Core.Dependencies.Interception
{
    /// <summary>
    /// Represents a result of method invocation
    /// </summary>
    public class MethodReturn: IMethodReturn
    {
        private readonly object _returnValue;
        public MethodReturn(object returnValue)
        {
            _returnValue = returnValue;
        }

        /// <summary>
        /// Returns return value of method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetReturnValue<T>()
        {
            if (_returnValue == null)
                return default(T);
            return (T) _returnValue;
        }

        /// <summary>
        /// Exception which occured during method invocation
        /// </summary>
        public Exception Exception { get; set; }
        
    }
}
