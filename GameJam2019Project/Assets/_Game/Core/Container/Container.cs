using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Containers
{
    internal class Container : IContainer
    {
        /* ----------------------------------------------------------------------------  */
        /*                                  PROPERTIES                                   */
        /* ----------------------------------------------------------------------------  */
        private Dictionary<Type, TypeFactory> _types = new Dictionary<Type, TypeFactory>();

        /* ----------------------------------------------------------------------------  */
        /*                                PUBLIC METHODS                                 */
        /* ----------------------------------------------------------------------------  */
        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        /* ----------------------------------------------------------------------------  */
        /*                               INTERNAL METHODS                                */
        /* ----------------------------------------------------------------------------  */
        internal void Register(Type key, Type value) => Register(key, value, null, false);
        internal void Register(Type key, Func<IContainer, object> factory) => Register(key, null, factory, false);
        internal void RegisterSingleton(Type key, Type value) => Register(key, value, null, true);
        internal void RegisterSingleton(Type key, Func<IContainer, object> factory) => Register(key, null, factory, true);

        internal bool TryResolve(Type type, out object result)
        {
            TypeFactory iocFactory;

            //Try and get specific type
            if (!_types.TryGetValue(type, out iocFactory))
            { }

            //Try and get derived type
            if (iocFactory == null)
            {
                foreach (KeyValuePair<Type, TypeFactory> pair in _types)
                {
                    var interfaces = pair.Key.GetInterfaces();
                    if (interfaces.Contains(type))
                    {
                        iocFactory = pair.Value;
                        break;
                    }

                }
            }

            //Try and create result
            result = iocFactory?.Create();
            return result != null;
        }

        internal object Resolve(Type type)
        {
            object result;
            bool success = TryResolve(type, out result);

            if (!success)
                throw new Exception($"{type.Name} not found in container");

            return result;
        }

        /* ----------------------------------------------------------------------------  */
        /*                                PRIVATE METHODS                                */
        /* ----------------------------------------------------------------------------  */
        private void Register(Type key, Type value, Func<IContainer, object> factory, bool isSingleton = false)
        {
            _types.Add(key, new TypeFactory(value ?? key, factory, this, isSingleton));
        }

        /* ----------------------------------------------------------------------------  */
        /*                               INTERNAL CLASSES                                */
        /* ----------------------------------------------------------------------------  */
        private class TypeFactory
        {
            private readonly Type _type;
            private readonly Func<IContainer, object> _factory;
            private readonly Container _container;

            private bool _isSingleton = false;
            private object _instance;
            private object _theLock = new object();

            public TypeFactory(Type type, Func<IContainer, object> factory, Container container, bool isSingleton = false)
            {
                _type = type;
                _factory = factory;
                _container = container;
                _isSingleton = isSingleton;
            }

            public virtual object Create()
            {
                if (_isSingleton)
                {
                    lock (_theLock)
                    {
                        if (_instance == null)
                            _instance = CreateType();
                        return _instance;
                    }
                }
                else
                {
                    return CreateType();
                }
            }

            private object CreateType()
            {
                if (_factory != null)
                {
                    return _factory.Invoke(_container);
                }
                else
                {
                    ConstructorInfo[] constructorsInfo = _type.GetConstructors();

                    if (constructorsInfo.Length > 1)
                        throw new Exception("Can not use more than on constructor");

                    ConstructorInfo constructorInfo = constructorsInfo.First();

                    ParameterInfo[] parametersInfo = constructorInfo.GetParameters();

                    IList<object> parameters = new List<object>();

                    //Resolve stuff
                    foreach (ParameterInfo parameterInfo in parametersInfo)
                    {
                        object result;
                        _container.TryResolve(parameterInfo.ParameterType, out result);
                        parameters.Add(result);
                    }

                    return constructorInfo.Invoke(parameters.ToArray());
                }
            }
        }
    }
}
