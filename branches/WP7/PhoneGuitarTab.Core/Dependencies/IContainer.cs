namespace PhoneGuitarTab.Core.Dependencies
{
    using System;
    using System.Collections.Generic;

    using PhoneGuitarTab.Core.Dependencies.Lifetime;

    /// <summary>
    ///  Represents dependency injection container behavior
    ///  </summary>
    public interface IContainer: IDisposable
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

        #region Register Type

        IContainer RegisterType(Type t, Type c);
        IContainer RegisterType(Type t, Type c, string name);
        IContainer RegisterType(Type t, Type c, ILifetimeManager lifetimeManager);
        IContainer RegisterType(Type t, Type c, string name, ILifetimeManager lifetimeManager);
        IContainer RegisterType(Type t, Type c, params object[] args);
        IContainer RegisterType(Type t, Type c, string  name, params object[] args);
        IContainer RegisterType(Type t, Type c, ILifetimeManager lifetimeManager, params object[] args);
        IContainer RegisterType(Type t, Type c, string name, ILifetimeManager lifetimeManager, params object[] args);
        IContainer RegisterType<T, C>() where C : class, T;
        IContainer RegisterType<T, C>(string name) where C : class, T;
        IContainer RegisterType<T, C>(ILifetimeManager lifetimeManager) where C : class, T;
        IContainer RegisterType<T, C>(string name, ILifetimeManager lifetimeManager) where C : class, T;
        IContainer RegisterType<T, C>(params object[] args) where C : class, T;
        IContainer RegisterType<T, C>(string name, params object[] args) where C : class, T;
        IContainer RegisterType<T, C>(ILifetimeManager lifetimeManager, params object[] args) where C : class, T;
        IContainer RegisterType<T, C>(string name, ILifetimeManager lifetimeManager, params object[] args) where C : class, T;
        IContainer RegisterType(Type t);
        IContainer RegisterType(Type t, string name);
        IContainer RegisterType(Type t, ILifetimeManager lifetimeManager);
        IContainer RegisterType(Type t, string name, ILifetimeManager lifetimeManager);
        IContainer RegisterType(Type t, params object[] args);
        IContainer RegisterType(Type t, string name, params object[] args);
        IContainer RegisterType(Type t, ILifetimeManager lifetimeManager, params object[] args);
        IContainer RegisterType(Type t, string name, ILifetimeManager lifetimeManager, params object[] args);
        IContainer RegisterType<T>() where T : class;
        IContainer RegisterType<T>(string name) where T : class;
        IContainer RegisterType<T>(ILifetimeManager lifetimeManager) where T : class;
        IContainer RegisterType<T>(string name, ILifetimeManager lifetimeManager) where T : class;
        IContainer RegisterType<T>(params object[] args) where T : class;
        IContainer RegisterType<T>(string name, params object[] args) where T : class;
        IContainer RegisterType<T>(ILifetimeManager lifetimeManager, params object[] args) where T : class;
        IContainer RegisterType<T>(string name, ILifetimeManager lifetimeManager, params object[] args) where T : class;

        #endregion

        #region Register instance

        IContainer RegisterInstance<T>(T instance);
        IContainer RegisterInstance<T>(T instance, string name);
        IContainer RegisterInstance(Type t, object instance);
        IContainer RegisterInstance(Type t, object instance, string name);

        #endregion
    }
}
