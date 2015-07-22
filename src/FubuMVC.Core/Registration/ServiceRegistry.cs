using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Registration
{
    public interface IServiceRegistration
    {
        void Apply(ServiceGraph services);
    }


    public class ServiceRegistry : IServiceRegistration
    {
        private readonly IList<Action<ServiceGraph>> _alterations = new List<Action<ServiceGraph>>();

        // Yes, I know this makes Dru go haywire, but he can come back in and
        // make the code uglier just to satisfy his own tastes
        private Action<ServiceGraph> alter
        {
            set { _alterations.Add(value); }
        }

        void IServiceRegistration.Apply(ServiceGraph services)
        {
            _alterations.Each(x => x(services));
        }

        /// <summary>
        /// Sets the instanceault implementation of a service if there is no
        /// previous registration
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public void SetServiceIfNone<TService, TImplementation>() where TImplementation : TService
        {
            fill(typeof (TService), new ConfiguredInstance(typeof (TImplementation)));
        }


        /// <summary>
        /// Sets the instanceault implementation of a service if there is no
        /// previous registration
        /// </summary>
        public void SetServiceIfNone(Type type, Instance instance)
        {
            fill(type, instance);
        }

        /// <summary>
        /// Sets the instanceault implementation of a service if there is no
        /// previous registration, and allows you to customize the Instance created
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public void SetServiceIfNone<TService, TImplementation>(Action<Instance> configure) where TImplementation : TService
        {
            var instance = new ConfiguredInstance(typeof (TImplementation));
            configure(instance);

            fill(typeof(TService), instance);
        }

        /// <summary>
        /// Sets the instanceault implementation of a service if there is no
        /// previous registration
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="value"></param>
        public void SetServiceIfNone<TService>(TService value)
        {
            fill(typeof (TService), new ObjectInstance(value));
        }

        /// <summary>
        /// Sets the instanceault implementation of a service if there is no
        /// previous registration
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="concreteType"></param>
        /// <returns></returns>
        public Instance SetServiceIfNone(Type interfaceType, Type concreteType)
        {
            var Instance = new ConfiguredInstance(concreteType);
            fill(interfaceType, Instance);
            return Instance;
        }

        /// <summary>
        /// Registers an *additional* implementation of a service.  Actual behavior varies by actual
        /// IoC container
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        public Instance AddService<TService, TImplementation>() where TImplementation : TService
        {
            var implementationType = typeof (TImplementation);
            return AddService<TService>(implementationType);
        }

        /// <summary>
        /// Registers an *additional* implementation of a service.  Actual behavior varies by actual
        /// IoC container
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="implementation"></param>
        /// <returns></returns>
        public Instance AddService<TService>(Type implementationType)
        {
            var Instance = new ConfiguredInstance(implementationType);

            alter = x => x.AddService(typeof (TService), Instance);

            return Instance;
        }

        /// <summary>
        /// Registers a instanceault implementation for a service.  Overwrites any existing
        /// registration
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public void ReplaceService<TService, TImplementation>() where TImplementation : TService
        {
            alter = x => x.Clear(typeof (TService));

            AddService<TService, TImplementation>();
        }

        /// <summary>
        /// Registers a instanceault implementation for a service.  Overwrites any existing
        /// registration
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="value"></param>
        public void ReplaceService<TService>(TService value)
        {
            alter = x => x.Clear(typeof (TService));
            AddService(value);
        }

        /// <summary>
        /// Registers an *additional* implementation of a service.  Actual behavior varies by actual
        /// IoC container
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="value"></param>
        public void AddService<TService>(TService value)
        {
            var instance = new ObjectInstance(value);

            alter = x => x.AddService(typeof (TService), instance);
        }

        /// <summary>
        /// Removes any registrations for type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddService(Type type, Instance Instance)
        {
            alter = x => x.AddService(type, Instance);
        }

        /// <summary>
        /// Registers the concreteType against the interfaceType
        /// if the registration does not already include the concreteType 
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="concreteType"></param>
        public void ClearAll<T>()
        {
            alter = x => x.Clear(typeof (T));
        }

        /// <summary>
        /// Registers an *additional* implementation of a service.  Actual behavior varies by actual
        /// IoC container
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Instance"></param>
        public void FillType(Type interfaceType, Type concreteType)
        {
            alter = x => x.FillType(interfaceType, concreteType);
        }

        private void fill(Type serviceType, Instance instance)
        {
            alter = x => x.SetServiceIfNone(serviceType, instance);
        }

        public static bool ShouldBeSingleton(Type type)
        {
            if (type == null) return false;

            return type.Name.EndsWith("Cache") || type.HasAttribute<SingletonAttribute>();
        }

        public void ReplaceService(Type type, Instance @instanceault)
        {
            alter = x =>
            {
                x.Clear(type);
                x.AddService(type, @instanceault);
            };
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SingletonAttribute : Attribute
    {
    }
}