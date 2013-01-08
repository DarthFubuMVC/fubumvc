using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Util;

namespace FubuMVC.Core.Runtime
{
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
        
        IEnumerable<ConvertProblem> ProblemsFor<T>();

        /// <summary>
        /// Attempts to find the first existing object in this FubuRequest that can be cast to
        /// type T.  Can return null.
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


    public class InMemoryFubuRequest : IFubuRequest
    {
        private readonly Cache<Type, object> _cache = new Cache<Type, object>(onMissing => null);

        public T Get<T>() where T : class
        {
            return _cache[typeof (T)] as T;
        }

    	public object Get(Type type)
    	{
    		return _cache[type];
    	}

    	public void Set<T>(T target) where T : class
        {
            _cache[typeof (T)] = target;
        }

        public void Set(Type type, object target)
        {
            _cache[type] = target;
        }

        public IEnumerable<ConvertProblem> ProblemsFor<T>()
        {
            return new ConvertProblem[0];
        }

        public IEnumerable<T> Find<T>() where T : class
        {
            return _cache.GetAll().OfType<T>();
        }

        public bool Has<T>()
        {
            return _cache.Has(typeof(T));
        }

        public bool Has(Type type)
        {
            return _cache.Has(type);
        }

        public void SetObject(object input)
        {
            if (input == null) throw new ArgumentNullException();

            _cache[input.GetType()] = input;
        }

        public void Clear(Type getType)
        {
            if (_cache.Has(getType))
            {
                _cache.Remove(getType);
            }
        }
    }
}