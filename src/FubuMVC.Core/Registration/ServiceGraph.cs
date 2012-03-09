using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Registration.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuCore;

namespace FubuMVC.Core.Registration
{
    public class ServiceGraph : TracedNode
    {
        private readonly Cache<Type, List<ObjectDef>> _services =
            new Cache<Type, List<ObjectDef>>(t => new List<ObjectDef>());

        public void AddService(Type serviceType, ObjectDef def)
        {
            Trace(new ServiceAdded(serviceType, def));
            _services[serviceType].Add(def);
        }

        public void AddService<TService, TImplementation>() where TImplementation : TService
        {
            AddService(typeof(TService), ObjectDef.ForType<TImplementation>());
        }

        public void AddService<TService>(TService instance)
        {
            AddService(typeof(TService), ObjectDef.ForValue(instance));
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

            AddService(interfaceType, new ObjectDef(concreteType));
        }

        public void SetServiceIfNone(Type serviceType, ObjectDef def)
        {
            List<ObjectDef> list = _services[serviceType];
            if (list.Any()) return;

            AddService(serviceType, def);
        }

        public void SetServiceIfNone(Type serviceType, Type implementationType)
        {
            SetServiceIfNone(serviceType, new ObjectDef(implementationType));
        }

        public void SetServiceIfNone<TInterface, TImplementation>()
        {
            SetServiceIfNone(typeof(TInterface), ObjectDef.ForType<TImplementation>());
        }

        /// <summary>
        /// Returns the currently registered default registration for
        /// the given TService
        /// </summary>
        /// <returns></returns>
        public ObjectDef DefaultServiceFor(Type serviceType)
        {
            return _services[serviceType].FirstOrDefault();
        }

        public ObjectDef DefaultServiceFor<T>()
        {
            return DefaultServiceFor(typeof (T));
        }

        /// <summary>
        /// Retrieves all the registered ObjectDef's for the specified type
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public IEnumerable<ObjectDef> ServicesFor(Type serviceType)
        {
            return _services[serviceType];
        }

        public IEnumerable<ObjectDef> ServicesFor<T>()
        {
            return ServicesFor(typeof(T));
        }

        public void Each(Action<Type, ObjectDef> action)
        {
            _services.Each((t, list) => list.Each(def => action(t, def)));
        }

        public void Configure<T>(Action<T> configure) where T : class, new()
        {
            var list = _services[typeof (T)];
            T @object = null;
            list.FirstOrDefault().IfNotNull(x => @object = x.Value as T);

            if (@object == null)
            {
                list.Clear();
                @object = new T();
                list.Add(ObjectDef.ForValue(@object));
            }

            configure(@object);
        }

        /// <summary>
        /// Returns an enumeration of all explicitly registered objects
        /// of the type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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