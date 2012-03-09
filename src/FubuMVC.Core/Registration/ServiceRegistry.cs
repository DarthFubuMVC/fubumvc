using System;
using System.Collections.Generic;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration
{
    public class ServiceRegistry : IServiceRegistry
    {
        private readonly ServiceGraph _graph = new ServiceGraph();

        public void SetServiceIfNone<TService, TImplementation>() where TImplementation : TService
        {
            fill(typeof (TService), new ObjectDef(typeof (TImplementation)));
        }

        public void SetServiceIfNone<TService>(TService value)
        {
            fill(typeof (TService), new ObjectDef{
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

            _graph.Add(typeof (TService), objectDef);

            return objectDef;
        }

        public void ReplaceService<TService, TImplementation>() where TImplementation : TService
        {
            _graph.Clear(typeof (TService));

            AddService<TService, TImplementation>();
        }

        public void ReplaceService<TService>(TService value)
        {
            _graph.Clear(typeof (TService));
            AddService(value);
        }

        public void AddService<TService>(TService value)
        {
            var objectDef = new ObjectDef{
                Value = value
            };

            _graph.Add(typeof (TService), objectDef);
        }

        public void AddService(Type type, ObjectDef objectDef)
        {
            _graph.Add(type, objectDef);
        }

        [WannaKill("Try to eliminate this")]
        public ObjectDef DefaultServiceFor<TService>()
        {
            return _graph.DefaultServiceFor(typeof (TService));
        }

        [WannaKill("Try to eliminate this")]
        public ObjectDef DefaultServiceFor(Type serviceType)
        {
            return _graph.DefaultServiceFor(serviceType);
        }

        [WannaKill("Try to eliminate this")]
        public IEnumerable<ObjectDef> ServicesFor<TService>()
        {
            return ServicesFor(typeof (TService));
        }

        [WannaKill("Try to eliminate this")]
        public IEnumerable<ObjectDef> ServicesFor(Type serviceType)
        {
            return _graph.ServicesFor(serviceType);
        }

        [WannaKill("Move")]
        public void Each(Action<Type, ObjectDef> action)
        {
            _graph.Each(action);
        }

        [WannaKill("Try to eliminate this")]
        public IEnumerable<T> FindAllValues<T>()
        {
            return _graph.FindAllValues<T>();
        }

        public void ClearAll<T>()
        {
            _graph.Clear(typeof (T));
        }

        public void FillType(Type interfaceType, Type concreteType)
        {
            _graph.FillType(interfaceType, concreteType);
        }

        private void fill(Type serviceType, ObjectDef def)
        {
            _graph.SetServiceIfNone(serviceType, def);
        }

        public static bool ShouldBeSingleton(Type type)
        {
            return type.Name.EndsWith("Cache") || type.HasAttribute<SingletonAttribute>();
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SingletonAttribute : Attribute
    {
    }
}