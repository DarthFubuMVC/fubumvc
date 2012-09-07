using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration
{
    [CanBeMultiples]
    public class ServiceRegistry : IServiceRegistry, IConfigurationAction
    {
        private readonly IList<Action<ServiceGraph>> _alterations = new List<Action<ServiceGraph>>();

        // Yes, I know this makes Dru go haywire, but he can come back in and
        // make the code uglier just to satisfy his own tastes
        private Action<ServiceGraph> alter
        {
            set { _alterations.Add(value); }
        }

        void IConfigurationAction.Configure(BehaviorGraph graph)
        {
            _alterations.Each(x => x(graph.Services));
        }


        public void SetServiceIfNone<TService, TImplementation>() where TImplementation : TService
        {
            fill(typeof (TService), new ObjectDef(typeof (TImplementation)));
        }

        public void SetServiceIfNone<TService, TImplementation>(Action<ObjectDef> configure) where TImplementation : TService
        {
            var def = new ObjectDef(typeof (TImplementation));
            configure(def);

            fill(typeof(TService), def);
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

            alter = x => x.AddService(typeof (TService), objectDef);

            return objectDef;
        }

        public void ReplaceService<TService, TImplementation>() where TImplementation : TService
        {
            alter = x => x.Clear(typeof (TService));

            AddService<TService, TImplementation>();
        }

        public void ReplaceService<TService>(TService value)
        {
            alter = x => x.Clear(typeof (TService));
            AddService(value);
        }

        public void AddService<TService>(TService value)
        {
            var objectDef = new ObjectDef{
                Value = value
            };

            alter = x => x.AddService(typeof (TService), objectDef);
        }

        public void AddService(Type type, ObjectDef objectDef)
        {
            alter = x => x.AddService(type, objectDef);
        }

        public void ClearAll<T>()
        {
            alter = x => x.Clear(typeof (T));
        }

        public void FillType(Type interfaceType, Type concreteType)
        {
            alter = x => x.FillType(interfaceType, concreteType);
        }

        public void ConfigureRequirements(Action<BottleConfigurationDef> action)
        {
            var def = new BottleConfigurationDef(GetType().Namespace);
            action(def);

            alter = x => def.As<IServiceGraphAlteration>().Alter(x);
        }

        private void fill(Type serviceType, ObjectDef def)
        {
            alter = x => x.SetServiceIfNone(serviceType, def);
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