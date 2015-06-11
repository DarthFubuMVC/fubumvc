using System;
using System.Collections.Generic;
using FubuCore.Binding;

namespace FubuMVC.Core.Runtime
{
    // SAMPLE: ifuburequest
    /// <summary>
    /// Primary model bag scoped per request.  IFubuRequest is the primary way to share state between behaviors
    /// in a running behavior chain
    /// </summary>
    public interface IFubuRequest
    {
        /// <summary>
        /// Find a model of the exact type T.  If there is not already a known instance of T, IFubuRequest will attempt to
        /// use model binding against the current request data to resolve a T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>() where T : class;

        /// <summary>
        /// Find a model of the exact type.  If there is not already a known instance of T, IFubuRequest will attempt to
        /// use model binding against the current request data to resolve a T
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
    	    object Get(Type type);

        /// <summary>
        /// Stores a value of type T in the FubuRequest
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        void Set<T>(T target) where T : class;

        /// <summary>
        /// Stores a value of type T in the FubuRequest
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        void Set(Type type, object target);
        
        /// <summary>
        /// Find any and all binding conversion problems
        /// found while model binding the current
        /// value of the type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<ConvertProblem> ProblemsFor<T>();

        /// <summary>
        /// Attempts to find all the existing objects in this FubuRequest that can be cast to
        /// type T.  Can return an empty enumerable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> Find<T>() where T : class;

        /// <summary>
        /// Has an object of type T already been resolved?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool Has<T>();

        /// <summary>
        /// Has an object of this type already been resolved?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool Has(Type type);

        /// <summary>
        /// Stores the object as object.GetType()
        /// </summary>
        /// <param name="input"></param>
        void SetObject(object input);

        /// <summary>
        /// Removes any object stored as Type type
        /// </summary>
        /// <param name="type"></param>
        void Clear(Type type);
    }
    // ENDSAMPLE
}