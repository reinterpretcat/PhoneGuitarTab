using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PhoneGuitarTab.Core.Dependencies.Interception.Proxies;
using PhoneGuitarTab.Core.Dependencies.Lifetime;

namespace PhoneGuitarTab.Core.Dependencies
{
    /// <summary>
    /// Represents dependency injection container
    /// </summary>
    public sealed class Container : IContainer
    {
        //TODO use specific type here
        private readonly Dictionary<Primitives.Tuple<string, Type>, ILifetimeManager> _typeMapping = new Dictionary<Primitives.Tuple<string, Type>, ILifetimeManager>();

        private readonly object[] _emptyArguments = new object[0];
        private readonly object _syncLock = new object();
        private readonly Type _lifetimeManager = typeof(SingletonLifetimeManager);
        private readonly string _defaultKey = String.Empty;

        #region IContainer implementation

        #region Resolve

        public object Resolve(string name)
        {
            var key = _typeMapping.Keys.Single(t => t.Item1 == name);
            return ResolveDependencies(ResolveLifetime( _typeMapping[key]).GetInstance(name));
        }

        public object Resolve(Type type)
        {
            return Resolve(type, _defaultKey);
        }

        public object Resolve(Type type, string name)
        {
            try
            {
                //try to find value using full key
                var key = new Primitives.Tuple<string, Type>(name, type);
                if(_typeMapping.ContainsKey(key))
                    return ResolveDependencies(ResolveLifetime(_typeMapping[key]).GetInstance());

                //try to find using only type and delegate resolving of instance by name to LifetimeManager that
                //can be useful in custom lifetime managers
                var altKey = _typeMapping.Keys.Single(k => k.Item2 == type);
                //inject container dependency here if attribute is specified
                return ResolveDependencies(ResolveLifetime(_typeMapping[altKey]).GetInstance(name));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("Unable to resolve type '{0}', name '{1}'", type, name), ex);
            }
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return ResolveAll(typeof (T)).Select(t => (T)t);
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            var keys = _typeMapping.Keys.Where(k => k.Item2 == type);
            return keys.Select(key => ResolveDependencies(ResolveLifetime(_typeMapping[key]).GetInstance(key.Item1)));
        }


        private ILifetimeManager ResolveLifetime(ILifetimeManager lifetimeManager)
        {
            //if cstor isn't provided, try to resolve one with dependency attribute
            if (lifetimeManager.Constructor == null && lifetimeManager.TargetType != null)
                lifetimeManager.Constructor = TypeHelper.GetConstructor(lifetimeManager.TargetType, typeof (DependencyAttribute));

            //NOTE: resolve all parameters of provided constructor
            if (lifetimeManager.Constructor != null)
                lifetimeManager.CstorArgs = lifetimeManager.Constructor.GetParameters().Select(p=> Resolve(p.ParameterType)).ToArray();
            return lifetimeManager;
        }

        /// <summary>
        /// Injects dependencies via property
        /// </summary>
        private object ResolveDependencies(object instance)
        {
            //if type's methods are intercepted, instance is proxy and doesn't have properties to do DI
            object proxy = null;
            if (instance is IProxy)
            {
                proxy = instance;
                instance = (instance as IProxy).Instance;
            }

            //Try to resolve property dependency injection
            Type objectType = instance.GetType();

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var properties = objectType.GetProperties(flags).ToList();
            var baseType = objectType.BaseType;
            while (baseType != null)
            {
                properties.AddRange(baseType.GetProperties(flags));
                baseType = baseType.BaseType;
            }

            foreach (PropertyInfo property in properties.Distinct())
            {
                foreach (DependencyAttribute attribute in property.GetCustomAttributes(typeof(DependencyAttribute), true))
                {
                    var propertyType = property.PropertyType;
                    object value;
                    //special case
                    if (propertyType == typeof(IContainer) || propertyType == typeof(Container))
                        value = this;
                    else
                    {
                        //resolve type from container
                        var registeredName = attribute.Name;
                        value = !String.IsNullOrEmpty(registeredName) ? 
                            Resolve(propertyType, registeredName) : 
                            Resolve(propertyType);
                    }
                    //set value to target property
                    property.SetValue(instance, value, null);
                   // objectType.InvokeMember(property.Name, BindingFlags.Public | BindingFlags.NonPublic | 
                   //     BindingFlags.SetProperty | BindingFlags.Instance, 
                   //     null, instance, new [] { value });
                }
            }
            return proxy ?? instance;
        }

        public T Resolve<T>()
        {
            return (T) Resolve(typeof (T));
        }

        public T Resolve<T>(string name)
        {
            return (T) Resolve(typeof(T), name);
        }


        #endregion

        #region Register component

        /// <summary>
        /// Registers Component
        /// </summary>
        public IContainer Register(Component component)
        {
            var lifetimeManager =  component.LifetimeManager ?? Activator.CreateInstance(_lifetimeManager) as ILifetimeManager;
            lifetimeManager.Constructor = component.Constructor;
            return RegisterType(component.InterfaceType, component.TargetType, component.Name,
                                lifetimeManager,
                                component.Args ?? _emptyArguments);
        }

        #endregion

        #region Register type

        /// <summary>
        /// Registers instance of <see cref="C"/> as <see cref="T"/>
        /// </summary>
        public IContainer RegisterType<T, C>() where C : class, T
        {
            return RegisterType<T, C>(_emptyArguments);
        }

        /// <summary>
        /// Registers type using name
        /// </summary>
        public IContainer RegisterType<T, C>(string name) where C : class, T
        {
            return RegisterType<T, C>(name, _emptyArguments);
        }

        /// <summary>
        /// Registers type using type manager provided
        /// </summary>
        public IContainer RegisterType<T, C>(ILifetimeManager lifetimeManager) where C : class, T
        {
            return RegisterType<T, C>(lifetimeManager, _emptyArguments);
        }

        /// <summary>
        /// Registers type using name
        /// </summary>
        public IContainer RegisterType<T, C>(string name, ILifetimeManager lifetimeManager) where C : class, T
        {
            return RegisterType<T, C>(name, lifetimeManager, _emptyArguments);
        }

        /// <summary>
        /// Registers instance of <see cref="C"/> as <see cref="T"/>
        /// </summary>
        public IContainer RegisterType<T, C>(object[] args) where C : class, T
        {
            return RegisterType(typeof(T), typeof(C), args);
        }

        /// <summary>
        /// Registers type using name
        /// </summary>
        public IContainer RegisterType<T, C>(string name, object[] args) where C : class, T
        {
            return RegisterType(typeof(T), typeof(C), name, args);
        }

        /// <summary>
        /// Registers type using type manager provided
        /// </summary>
        public IContainer RegisterType<T, C>(ILifetimeManager lifetimeManager, object[] args) where C : class, T
        {
            return RegisterType(typeof(T), typeof(C), lifetimeManager, args);
        }

        /// <summary>
        /// Registers type using name
        /// </summary>
        public IContainer RegisterType<T, C>(string name, ILifetimeManager lifetimeManager, object[] args) where C : class, T
        {
            return RegisterType(typeof(T), typeof(C), name, lifetimeManager, args);
        }

        /// <summary>
        /// Registers instance of  <see cref="T"/>
        /// </summary>
        public IContainer RegisterType<T>() where T : class
        {
            return RegisterType<T>(_emptyArguments);
        }

        /// <summary>
        /// Registers type using name
        /// </summary>
        public IContainer RegisterType<T>(string name) where T : class
        {
            return RegisterType<T>(name, _emptyArguments);
        }


        /// <summary>
        /// Registers type using type manager provided
        /// </summary>
        public IContainer RegisterType<T>(ILifetimeManager lifetimeManager) where T : class
        {
            return RegisterType<T>(lifetimeManager, _emptyArguments);
        }

        /// <summary>
        /// Registers type using name
        /// </summary>
        public IContainer RegisterType<T>(string name, ILifetimeManager lifetimeManager) where T : class
        {
            return RegisterType<T>(name, lifetimeManager, _emptyArguments);
        }

        /// <summary>
        /// Registers instance of  <see cref="T"/>
        /// </summary>
        public IContainer RegisterType<T>(object[] args) where T : class
        {
            return RegisterType(typeof(T), args);
        }

        /// <summary>
        /// Registers type using name
        /// </summary>
        public IContainer RegisterType<T>(string name, object[] args) where T : class
        {
            return RegisterType(typeof (T), name, args);
        }


        /// <summary>
        /// Registers type using type manager provided
        /// </summary>
        public IContainer RegisterType<T>(ILifetimeManager lifetimeManager, object[] args) where T : class
        {
            return RegisterType(typeof(T), lifetimeManager, args);
        }

        /// <summary>
        /// Registers type using name
        /// </summary>
        public IContainer RegisterType<T>(string name, ILifetimeManager lifetimeManager, object[] args) where T : class
        {
            return RegisterType(typeof(T), name, lifetimeManager, args);
        }

        /// <summary>
        /// Registers the instance of type t
        /// </summary>
        public IContainer RegisterType(Type t, Type c)
        {
            return RegisterType(t, c, _emptyArguments);
        }

        /// <summary>
        /// Registers type using name
        /// </summary>
        public IContainer RegisterType(Type t, Type c, string name)
        {
            return RegisterType(t, c, name, _emptyArguments);
        }

        /// <summary>
        /// Registers type using type manager provided
        /// </summary>
        public IContainer RegisterType(Type t, Type c, ILifetimeManager lifetimeManager)
        {
            return RegisterType(t, c, lifetimeManager, _emptyArguments);
        }

        /// <summary>
        /// Registers type using name
        /// </summary>
        public IContainer RegisterType(Type t, Type c, string name, ILifetimeManager lifetimeManager)
        {
            return RegisterType(t, c, name, lifetimeManager, _emptyArguments);
        }

        /// <summary>
        /// Registers the instance of type t
        /// </summary>
        public IContainer RegisterType(Type t, Type c, object[] args)
        {
            return RegisterType(t, c, _defaultKey, args);
        }

        /// <summary>
        /// Registers type using name
        /// </summary>
        public IContainer RegisterType(Type t, Type c, string name, object[] args)
        {
            return RegisterType(t, c, name, (Activator.CreateInstance(_lifetimeManager) as ILifetimeManager), args);
        }

        /// <summary>
        /// Registers type using type manager provided
        /// </summary>
        public IContainer RegisterType(Type t, Type c, ILifetimeManager lifetimeManager, object[] args)
        {
            return RegisterType(t, c, _defaultKey, lifetimeManager, args);
        }

        /// <summary>
        /// Registers type using name
        /// </summary>
        public IContainer RegisterType(Type t, Type c, string name, ILifetimeManager lifetimeManager, object[] args)
        {
            lifetimeManager.CstorArgs = args;
            lifetimeManager.TargetType = c;
            lifetimeManager.InterfaceType = t;
            lock (_syncLock)
                _typeMapping.Add(new Primitives.Tuple<string, Type>(name, t), lifetimeManager);
            return this;
        }

        /// <summary>
        /// Registers the instance of type t
        /// </summary>
        public IContainer RegisterType(Type t)
        {
            return RegisterType(t, _emptyArguments);
        }

        /// <summary>
        /// Registers type using name
        /// </summary>
        public IContainer RegisterType(Type t, string name)
        {
            return RegisterType(t, name, _emptyArguments);
        }

        /// <summary>
        /// Registers type using type manager provided
        /// </summary>
        public IContainer RegisterType(Type t, ILifetimeManager lifetimeManager)
        {
            return RegisterType(t, lifetimeManager, _emptyArguments);
        }

        /// <summary>
        /// Registers type using name
        /// </summary>
        public IContainer RegisterType(Type t, string name, ILifetimeManager lifetimeManager)
        {
            return RegisterType(t, name, lifetimeManager, _emptyArguments);
        }

        /// <summary>
        /// Registers the instance of type t
        /// </summary>
        public IContainer RegisterType(Type t, object[] args)
        {
            return RegisterType(t, t, args);
        }

        /// <summary>
        /// Registers type using name
        /// </summary>
        public IContainer RegisterType(Type t, string name, object[] args)
        {
            return RegisterType(t, t, name, args);
        }

        /// <summary>
        /// Registers type using type manager provided
        /// </summary>
        public IContainer RegisterType(Type t,ILifetimeManager lifetimeManager, object[] args)
        {
            return RegisterType(t, t, lifetimeManager, args);
        }

        public IContainer RegisterType(Type t, string name, ILifetimeManager lifetimeManager, object[] args)
        {
            return RegisterType(t, t, name, lifetimeManager, args);
        }

        #endregion

        #region Register instance

        /// <summary>
        /// Registers existing instance of type T
        /// </summary>
        public IContainer RegisterInstance<T>(T instance)
        {
            return RegisterInstance(typeof(T), instance);
        }

        public IContainer RegisterInstance<T>(T instance, string name)
        {
            return RegisterInstance(typeof(T), instance as object, name as string);
        }

        /// <summary>
        /// Registers existing instance of type t
        /// </summary>
        public IContainer RegisterInstance(Type t, object instance)
        {
            return RegisterInstance(t, instance, _defaultKey);
        }

        public IContainer RegisterInstance(Type t, object instance, string name)
        {
            //TODO: check whether the type is already registred
            lock (_syncLock)
                _typeMapping.Add(new Primitives.Tuple<string, Type>(name, t), new ExternalLifetimeManager(instance));
            return this;
        }

        #endregion

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            _typeMapping.Keys.ToList().ForEach(key => _typeMapping[key].Dispose());
        }

        #endregion
    }
}
