using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration
{
    public class ServiceRegistry : IServiceRegistry
    {
        private readonly Cache<Type, List<ObjectDef>> _services =
            new Cache<Type, List<ObjectDef>>(t => new List<ObjectDef>());

        public void SetServiceIfNone<TService, TImplementation>() where TImplementation : TService
        {
            fill(typeof (TService), new ObjectDef(typeof (TImplementation)));
        }

        public void SetServiceIfNone<TService>(TService value)
        {
            fill(typeof (TService), new ObjectDef
            {
                Value = value
            });
        }

        public ObjectDef SetServiceIfNone(Type interfaceType, Type concreteType)
        {
            var objectDef = new ObjectDef(concreteType);
            fill(interfaceType, objectDef);
            return objectDef;
        }

        public ObjectDef AddService<TService, TImplementation>() where TImplementation : TService
        {
            var implementationType = typeof (TImplementation);
            return AddService<TService>(implementationType);
        }

        public ObjectDef AddService<TService>(Type implementationType)
        {
            var objectDef = new ObjectDef(implementationType);
            _services[typeof (TService)].Add(objectDef);

            return objectDef;
        }

        public void ReplaceService<TService, TImplementation>() where TImplementation : TService
        {
            _services[typeof (TService)].Clear();
            AddService<TService, TImplementation>();
        }

        public void ReplaceService<TService>(TService value)
        {
            _services[typeof (TService)].Clear();
            AddService(value);
        }

        public void AddService<TService>(TService value)
        {
            var objectDef = new ObjectDef
            {
                Value = value
            };
            _services[typeof (TService)].Add(objectDef);
        }

        public void AddService(Type type, ObjectDef objectDef)
        {
            _services[type].Add(objectDef);
        }

        public ObjectDef DefaultServiceFor<TService>()
        {
            return _services[typeof (TService)].FirstOrDefault();
        }

        public ObjectDef DefaultServiceFor(Type serviceType)
        {
            return _services[serviceType].FirstOrDefault();
        }

        public IEnumerable<ObjectDef> ServicesFor<TService>()
        {
            return ServicesFor(typeof (TService));
        }

        public IEnumerable<ObjectDef> ServicesFor(Type serviceType)
        {
            return _services[serviceType];
        }

        public void Each(Action<Type, ObjectDef> action)
        {
            _services.Each((t, list) => list.Each(def => action(t, def)));
        }

        public IEnumerable<T> FindAllValues<T>()
        {
            foreach (ObjectDef def in _services[typeof (T)])
            {
                if (def.Value != null)
                {
                    yield return (T) def.Value;
                }
            }
        }

        public void ClearAll<T>()
        {
            _services[typeof (T)].Clear();
        }

        private void fill(Type serviceType, ObjectDef def)
        {
            List<ObjectDef> list = _services[serviceType];
            if (list.Any()) return;

            list.Add(def);
        }

        public void FillType(Type interfaceType, Type concreteType)
        {
            List<ObjectDef> list = _services[interfaceType];
            if (list.Any(x => x.Type == concreteType)) return;

            list.Add(new ObjectDef(concreteType));
        }

        public static bool ShouldBeSingleton(Type type, ObjectDef def)
        {
            return type.Name.EndsWith("Cache") &&
                (def.Type == null || !def.Type.Name.StartsWith("Recording"));
        }
    }
}