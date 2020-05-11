// © Xavalon. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xavalon.XamlStyler.Extension.Mac
{
    public class Container
    {
        private readonly Dictionary<Type, Lazy<object>> _storage;

        private Container()
        {
            _storage = new Dictionary<Type, Lazy<object>>();
        }

        public static Container Instance { get; } = new Container();

        public IInstance Resolve<IInstance>()
        {
            return (IInstance)_storage[typeof(IInstance)].Value;
        }

        public void LazyRegisterSingleton<IInstance, TInstance>() where TInstance : class, IInstance
        {
            var lazyInstance = new Lazy<object>(() =>
            {
                var constructorInfo = typeof(TInstance).GetConstructors(BindingFlags.Instance | BindingFlags.Public).Single();
                var constructorParameterInfos = constructorInfo.GetParameters();
                var constructorParameters = constructorParameterInfos.Select(parameter => parameter.ParameterType)
                                                                     .Select(type => _storage[type].Value)
                                                                     .ToArray();

                var instance = Activator.CreateInstance(typeof(TInstance), constructorParameters);
                return instance;
            });

            _storage[typeof(IInstance)] = lazyInstance;
        }
    }
}