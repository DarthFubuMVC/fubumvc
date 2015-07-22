using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Registration
{
    public class ServiceGraph
    {
        private readonly Cache<Type, List<Instance>> _services =
            new Cache<Type, List<Instance>>(t => new List<Instance>());

        public void AddService(Type serviceType, Instance def)
        {
            _services[serviceType].Add(def);
        }

        public void AddService<TService, TImplementation>() where TImplementation : TService
        {
            AddService(typeof (TService), new SmartInstance<TImplementation>());
        }

        public void AddService<TService>(TService instance)
        {
            AddService(typeof (TService), new ObjectInstance(instance));
        }

        public void Clear(Type serviceType)
        {
            var list = _services[serviceType];
            list.Clear();
        }

        public bool HasAny(Type type)
        {
            return _services[type].Any();
        }

        public void FillType(Type interfaceType, Type concreteType)
        {
            var list = _services[interfaceType];
            if (list.Any(x => x.ReturnedType == concreteType)) return;

            AddService(interfaceType, new ConfiguredInstance(concreteType));
        }

        public void SetServiceIfNone(Type serviceType, Instance def)
        {
            var list = _services[serviceType];
            if (list.Any()) return;

            AddService(serviceType, def);
        }

        public void SetServiceIfNone(Type serviceType, Type implementationType)
        {
            SetServiceIfNone(serviceType, new ConfiguredInstance(implementationType));
        }

        public void SetServiceIfNone<TInterface, TImplementation>()
        {
            SetServiceIfNone(typeof (TInterface), new SmartInstance<TImplementation>());
        }

        /// <summary>
        ///   Returns the currently registered default registration for
        ///   the given TService
        /// </summary>
        /// <returns></returns>
        public Instance DefaultServiceFor(Type serviceType)
        {
            return _services[serviceType].FirstOrDefault();
        }

        public Instance DefaultServiceFor<T>()
        {
            return DefaultServiceFor(typeof (T));
        }

        /// <summary>
        ///   Retrieves all the registered Instance's for the specified type
        /// </summary>
        /// <param name = "serviceType"></param>
        /// <returns></returns>
        public IEnumerable<Instance> ServicesFor(Type serviceType)
        {
            return _services[serviceType];
        }

        public IEnumerable<Instance> ServicesFor<T>()
        {
            return ServicesFor(typeof (T));
        }

        public void Each(Action<Type, Instance> action)
        {
            _services.Each((t, list) => list.Each(def => action(t, def)));
        }

    }
}