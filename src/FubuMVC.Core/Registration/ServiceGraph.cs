using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Registration.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration
{
    public class ServiceGraph : TracedNode
    {
        private readonly Cache<Type, List<ObjectDef>> _services =
            new Cache<Type, List<ObjectDef>>(t => new List<ObjectDef>());

        public void Add(Type serviceType, ObjectDef def)
        {
            Trace(new ServiceAdded(serviceType, def));
            _services[serviceType].Add(def);
        }

        public void Clear(Type serviceType)
        {
            var list = _services[serviceType];

            list.Each(def => Trace(new ServiceRemoved(serviceType, def)));

            list.Clear();
        }

        public bool HasAny(Type type)
        {
            return _services[type].Any();
        }

        public void FillType(Type interfaceType, Type concreteType)
        {
            List<ObjectDef> list = _services[interfaceType];
            if (list.Any(x => x.Type == concreteType)) return;

            Add(interfaceType, new ObjectDef(concreteType));
        }

        public void SetServiceIfNone(Type serviceType, ObjectDef def)
        {
            List<ObjectDef> list = _services[serviceType];
            if (list.Any()) return;

            Add(serviceType, def);
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

    public class ServiceAdded : NodeEvent
    {
        private readonly Type _serviceType;
        private readonly ObjectDef _def;

        public ServiceAdded(Type serviceType, ObjectDef def)
        {
            _serviceType = serviceType;
            _def = def;
        }

        public Type ServiceType
        {
            get { return _serviceType; }
        }

        public ObjectDef Def
        {
            get { return _def; }
        }

        public bool Equals(ServiceAdded other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._serviceType, _serviceType) && Equals(other._def, _def);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ServiceAdded)) return false;
            return Equals((ServiceAdded) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_serviceType != null ? _serviceType.GetHashCode() : 0)*397) ^ (_def != null ? _def.GetHashCode() : 0);
            }
        }
    }

    public class ServiceRemoved : NodeEvent
    {
        private readonly Type _serviceType;
        private readonly ObjectDef _def;

        public ServiceRemoved(Type serviceType, ObjectDef def)
        {
            _serviceType = serviceType;
            _def = def;
        }

        public Type ServiceType
        {
            get { return _serviceType; }
        }

        public ObjectDef Def
        {
            get { return _def; }
        }

        public bool Equals(ServiceRemoved other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._serviceType, _serviceType) && Equals(other._def, _def);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ServiceRemoved)) return false;
            return Equals((ServiceRemoved) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_serviceType != null ? _serviceType.GetHashCode() : 0)*397) ^ (_def != null ? _def.GetHashCode() : 0);
            }
        }
    }
}