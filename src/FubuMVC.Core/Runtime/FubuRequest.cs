using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Models;
using FubuMVC.Core.Util;

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

        public virtual void Set<T>(T target) where T : class
        {
            _values[typeof (T)] = new BindResult
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

        public virtual void SetObject(object input)
        {
            if (input == null) throw new ArgumentNullException();

            _values[input.GetType()] = new BindResult
            {
                Problems = new List<ConvertProblem>(),
                Value = input
            };
        }
    }
}