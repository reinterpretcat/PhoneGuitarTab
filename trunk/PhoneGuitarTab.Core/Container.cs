using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PhoneGuitarTab.Core
{
    public static class Container
    {
        #region Fields

        private static readonly Dictionary<Type, Tuple<Type, object[]>> _registration = new Dictionary<Type, Tuple<Type, object[]>>();
        private static readonly Dictionary<Type, object> _rot = new Dictionary<Type, object>();
        private static readonly object[] _emptyArguments = new object[0];
        private static readonly object _syncLock = new object();

        #endregion Fields


        #region Static constructor

        static Container()
        {
           
        }

        #endregion Static constructor


        #region Public methods

        public static object Resolve(Type type)
        {
            lock (_syncLock)
            {
                if (!_rot.ContainsKey(type))
                {
                    if (!_registration.ContainsKey(type))
                        throw new ArgumentException(String.Format("Type not registered: {0}", type));

                    Type resolveTo = _registration[type].Item1 ?? type;

                    ConstructorInfo constructor = resolveTo.GetConstructor(_registration[type].Item2.Select(a => a.GetType()).ToArray());
                    if (constructor == null)
                        throw new ArgumentException(String.Format("Unable to find appropriate constructor of type: {0}", resolveTo));
                    _rot[type] = constructor.Invoke(_registration[type].Item2);
                }
                return _rot[type];
            }
        }

        public static T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public static void Register<I, C>() where C : class, I
        {
            Register<I, C>(_emptyArguments);
        }

        public static void Register<I, C>(object[] args) where C : class, I
        {
            lock (_syncLock)
            {
                _registration.Add(typeof(I), new Tuple<Type, object[]>(typeof(C), args));
            }
        }

        public static void Register<C>() where C : class
        {
            Register<C>(_emptyArguments);
        }

        public static void Register<C>(object[] args) where C : class
        {
            lock (_syncLock)
            {
                _registration.Add(typeof(C), new Tuple<Type, object[]>(null, args));
            }
        }

        #endregion Public methods
    }
}
