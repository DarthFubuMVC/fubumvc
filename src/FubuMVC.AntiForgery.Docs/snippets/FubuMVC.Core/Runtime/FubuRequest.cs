using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Logging;
using FubuCore.Util;

namespace FubuMVC.Core.Runtime
{
    public class FubuRequest : IFubuRequest
    {
        private readonly ILogger _logger;
        private readonly Cache<Type, BindResult> _values = new Cache<Type, BindResult>();

        // SAMPLE: auto-resolving-within-ifuburequest
        public FubuRequest(IRequestData data, IObjectResolver resolver, ILogger logger)
        {
            _logger = logger;
            _values.OnMissing = (type => resolver.BindModel(type, data));
        }
        // ENDSAMPLE

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
            Set(typeof (T), target);
        }

        public void Set(Type type, object target)
        {
            _logger.DebugMessage(() => new SetValueReport{
                Type = type,
                Value = target
            });

            if (_values.Has(type) && ReferenceEquals(_values[type].Value, target))
            {
                return;
            }

            _values[type] = new BindResult{
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

            Set(input.GetType(), input);
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