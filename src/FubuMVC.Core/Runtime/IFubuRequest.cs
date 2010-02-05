using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Runtime
{
    public interface IFubuRequest
    {
        T Get<T>() where T : class;
        void Set<T>(T target) where T : class;
        IEnumerable<ConvertProblem> ProblemsFor<T>();
        IEnumerable<T> Find<T>() where T : class;
        void SetObject(object input);
    }


    public class InMemoryFubuRequest : IFubuRequest
    {
        private readonly Cache<Type, object> _cache = new Cache<Type, object>();

        public T Get<T>() where T : class
        {
            return _cache[typeof (T)] as T;
        }

        public void Set<T>(T target) where T : class
        {
            _cache[typeof (T)] = target;
        }

        public IEnumerable<ConvertProblem> ProblemsFor<T>()
        {
            return new ConvertProblem[0];
        }

        public IEnumerable<T> Find<T>() where T : class
        {
            return _cache.GetAll().OfType<T>();
        }

        public void SetObject(object input)
        {
            if (input == null) throw new ArgumentNullException();

            _cache[input.GetType()] = input;
        }
    }
}