using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Util;

namespace FubuMVC.Core.Runtime
{
    public class FubuRequest : IFubuRequest
    {
        private readonly Cache<Type, BindResult> _values = new Cache<Type, BindResult>();

        public FubuRequest(IRequestData data, IObjectResolver resolver)
        {
            _values.OnMissing = (type => resolver.BindModel(type, data));
        }

        public T Get<T>() where T : class
        {
            return _values[typeof (T)].Value as T;
        }

    	public object Get(Type type)
    	{
    		return _values[type].Value;
    	}

    	public virtual void Set<T>(T target) where T : class
        {
            _values[typeof (T)] = new BindResult
            {
                Value = target,
                Problems = new List<ConvertProblem>()
            };
        }

        public void Set(Type type, object target)
        {
            _values[type] = new BindResult()
            {
                Value = target,
                Problems = new List<ConvertProblem>()
            };
        }

        public IEnumerable<ConvertProblem> ProblemsFor<T>()
        {
            return _values[typeof (T)].Problems;
        }

        public IEnumerable<T> Find<T>() where T : class
        {
            return _values.GetAll().Select(x => x.Value).OfType<T>();
        }

        public bool Has<T>()
        {
            return _values.Has(typeof (T));
        }

        public bool Has(Type type)
        {
            return _values.Has(type);
        }

        public virtual void SetObject(object input)
        {
            if (input == null) throw new ArgumentNullException();

            _values[input.GetType()] = new BindResult
            {
                Problems = new List<ConvertProblem>(),
                Value = input
            };
        }

        public void Clear(Type getType)
        {
            if (_values.Has(getType))
            {
                _values.Remove(getType);
            }
        }
    }
}