using System;
using System.Collections.Generic;

namespace Core.Containers
{
    public class ContainerBuilder
    {
        private Dictionary<Type, TypeToRegisterWrapper> typesToRegister = new Dictionary<Type, TypeToRegisterWrapper>();

        /* ----------------------------------------------------------------------------  */
        /*                                PUBLIC METHODS                                 */
        /* ----------------------------------------------------------------------------  */
        public ContainerBuilder Register<T>() where T : class => Register(typeof(T), null, null, false);

        public ContainerBuilder Register<T1, T2>() where T1 : class where T2 : T1 => Register(typeof(T1), typeof(T2), null, false);

        public ContainerBuilder Register<T>(Func<IContainer, T> factory) where T : class => Register(typeof(T), null, factory, false);

        public ContainerBuilder RegisterSingleton<T>() where T : class => Register(typeof(T), null, null, true);

        public ContainerBuilder RegisterSingleton<T1, T2>() where T1 : class where T2 : T1 => Register(typeof(T1), typeof(T2), null, true);

        public ContainerBuilder RegisterSingleton<T>(Func<IContainer, T> factory) where T : class => Register(typeof(T), null, factory, true);

        public IContainer Build()
        {
            Container container = new Container();

            foreach (KeyValuePair<Type, TypeToRegisterWrapper> types in typesToRegister)
            {
                if (types.Value.IsSingleton)
                {
                    if (types.Value.Factory != null)
                        container.RegisterSingleton(types.Key, types.Value.Factory);
                    else
                        container.RegisterSingleton(types.Key, types.Value.Type);
                }
                else
                {
                    if (types.Value.Factory != null)
                        container.Register(types.Key, types.Value.Factory);
                    else
                        container.Register(types.Key, types.Value.Type);
                }

            }

            return container;
        }

        /* ----------------------------------------------------------------------------  */
        /*                                PRIVATE METHODS                                */
        /* ----------------------------------------------------------------------------  */
        private ContainerBuilder Register(Type type, Type implementation, Func<IContainer, object> factory, bool isSingleton)
        {
            typesToRegister.Add(type, new TypeToRegisterWrapper(implementation ?? type, factory, isSingleton));

            return this;
        }

        /* ----------------------------------------------------------------------------  */
        /*                               INTERNAL CLASSES                                */
        /* ----------------------------------------------------------------------------  */
        private class TypeToRegisterWrapper
        {
            public Type Type { get; }

            public Func<IContainer, object> Factory { get; }

            public bool IsSingleton { get; }

            public TypeToRegisterWrapper(Type type, Func<IContainer, object> factory, bool isSingleton)
            {
                Type = type;
                Factory = factory;
                IsSingleton = isSingleton;
            }
        }
    }
}
