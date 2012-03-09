using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration
{
    public interface IServiceRegistry
    {
        /// <summary>
        /// Sets the default implementation of a service if there is no
        /// previous registration
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        void SetServiceIfNone<TService, TImplementation>() where TImplementation : TService;

        /// <summary>
        /// Sets the default implementation of a service if there is no
        /// previous registration
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="value"></param>
        void SetServiceIfNone<TService>(TService value);

        /// <summary>
        /// Sets the default implementation of a service if there is no
        /// previous registration
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="concreteType"></param>
        /// <returns></returns>
        ObjectDef SetServiceIfNone(Type interfaceType, Type concreteType);

        /// <summary>
        /// Registers an *additional* implementation of a service.  Actual behavior varies by actual
        /// IoC container
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        ObjectDef AddService<TService, TImplementation>() where TImplementation : TService;

        /// <summary>
        /// Registers an *additional* implementation of a service.  Actual behavior varies by actual
        /// IoC container
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="implementation"></param>
        /// <returns></returns>
        ObjectDef AddService<TService>(Type implementation);

        /// <summary>
        /// Registers a default implementation for a service.  Overwrites any existing
        /// registration
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        void ReplaceService<TService, TImplementation>() where TImplementation : TService;
        
        /// <summary>
        /// Registers a default implementation for a service.  Overwrites any existing
        /// registration
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="value"></param>
        void ReplaceService<TService>(TService value);

        /// <summary>
        /// Registers an *additional* implementation of a service.  Actual behavior varies by actual
        /// IoC container
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="value"></param>
        void AddService<TService>(TService value);

        /// <summary>
        /// Removes any registrations for type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void ClearAll<T>();

        /// <summary>
        /// Registers the concreteType against the interfaceType
        /// if the registration does not already include the concreteType 
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="concreteType"></param>
        void FillType(Type interfaceType, Type concreteType);

        /// <summary>
        /// Registers an *additional* implementation of a service.  Actual behavior varies by actual
        /// IoC container
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objectDef"></param>
        void AddService(Type type, ObjectDef objectDef);
    }
}