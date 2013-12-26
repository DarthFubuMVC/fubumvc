using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Util;

namespace FubuMVC.Core.Runtime
{

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