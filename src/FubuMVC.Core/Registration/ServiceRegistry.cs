using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.ObjectGraph;

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
        /// Sets the default implementation of a service if there is no
        /// previous registration
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public void SetServiceIfNone<TService, TImplementation>() where TImplementation : TService
        {
            fill(typeof (TService), new ObjectDef(typeof (TImplementation)));
        }


        /// <summary>
        /// Sets the default implementation of a service if there is no
        /// previous registration
        /// </summary>
        public void SetServiceIfNone(Type type, ObjectDef def)
        {
            fill(type, def);
        }

        /// <summary>
        /// Sets the default implementation of a service if there is no
        /// previous registration, and allows you to customize the ObjectDef created
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public void SetServiceIfNone<TService, TImplementation>(Action<ObjectDef> configure) where TImplementation : TService
        {
            var def = new ObjectDef(typeof (TImplementation));
            configure(def);

            fill(typeof(TService), def);
        }

        /// <summary>
        /// Sets the default implementation of a service if there is no
        /// previous registration
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="value"></param>
        public void SetServiceIfNone<TService>(TService value)
        {
            fill(typeof (TService), new ObjectDef{
                Value = value
            });
        }

        /// <summary>
        /// Sets the default implementation of a service if there is no
        /// previous registration
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="concreteType"></param>
        /// <returns></returns>
        public ObjectDef SetServiceIfNone(Type interfaceType, Type concreteType)
        {
            var objectDef = new ObjectDef(concreteType);
            fill(interfaceType, objectDef);
            return objectDef;
        }

        /// <summary>
        /// Registers an *additional* implementation of a service.  Actual behavior varies by actual
        /// IoC container
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        public ObjectDef AddService<TService, TImplementation>() where TImplementation : TService
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
        public ObjectDef AddService<TService>(Type implementationType)
        {
            var objectDef = new ObjectDef(implementationType);

            alter = x => x.AddService(typeof (TService), objectDef);

            return objectDef;
        }

        /// <summary>
        /// Registers a default implementation for a service.  Overwrites any existing
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
        /// Registers a default implementation for a service.  Overwrites any existing
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
            var objectDef = new ObjectDef{
                Value = value
            };

            alter = x => x.AddService(typeof (TService), objectDef);
        }

        /// <summary>
        /// Removes any registrations for type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddService(Type type, ObjectDef objectDef)
        {
            alter = x => x.AddService(type, objectDef);
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
        /// <param name="objectDef"></param>
        public void FillType(Type interfaceType, Type concreteType)
        {
            alter = x => x.FillType(interfaceType, concreteType);
        }

        private void fill(Type serviceType, ObjectDef def)
        {
            alter = x => x.SetServiceIfNone(serviceType, def);
        }

        public static bool ShouldBeSingleton(Type type)
        {
            if (type == null) return false;

            return type.Name.EndsWith("Cache") || type.HasAttribute<SingletonAttribute>();
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SingletonAttribute : Attribute
    {
    }
}