using System;
using System.Collections.Generic;

namespace PhoneGuitarTab.Core.Dependencies
{
    /// <summary>
    ///     Represents dependency injection container behavior
    /// </summary>
    public interface IContainer : IDisposable
    {
        #region Resolve single instance

        object Resolve(Type type);
        object Resolve(Type type, string name);
        object Resolve(string name);
        T Resolve<T>();
        T Resolve<T>(string name);

        #endregion

        #region Resolve several instances

        IEnumerable<T> ResolveAll<T>();
        IEnumerable<object> ResolveAll(Type type);

        #endregion

        #region Register component

        IContainer Register(Component component);

        #endregion

        #region Register instance

        IContainer RegisterInstance<T>(T instance);
        IContainer RegisterInstance<T>(T instance, string name);
        IContainer RegisterInstance(Type t, object instance);
        IContainer RegisterInstance(Type t, object instance, string name);

        #endregion
    }
}