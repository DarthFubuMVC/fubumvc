using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration
{
    public class ServiceGraph
    {
        private readonly Cache<Type, List<ObjectDef>> _services =
            new Cache<Type, List<ObjectDef>>(t => new List<ObjectDef>());

        public void Add(Type type, ObjectDef def)
        {
            _services[type].Add(def);
        }

        public void Clear(Type type)
        {
            _services[type].Clear();
        }

        public bool HasAny(Type type)
        {
            return _services[type].Any();
        }

        public void FillType(Type interfaceType, Type concreteType)
        {
            List<ObjectDef> list = _services[interfaceType];
            if (list.Any(x => x.Type == concreteType)) return;

            list.Add(new ObjectDef(concreteType));
        }

        public void SetServiceIfNone(Type type, ObjectDef def)
        {
            List<ObjectDef> list = _services[type];
            if (list.Any()) return;

            list.Add(def);
        }

        public ObjectDef DefaultServiceFor(Type serviceType)
        {
            return _services[serviceType].FirstOrDefault();
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
            foreach (ObjectDef def in _services[typeof(T)])
            {
                if (def.Value != null)
                {
                    yield return (T)def.Value;
                }
            }
        }
    }
}