using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Util;
using FubuMVC.Core.Runtime.Logging;

namespace FubuMVC.Core.Runtime
{
    public class FubuRequest : IFubuRequest
    {
        private readonly ILogger _logger;
        private readonly Cache<Type, BindResult> _values = new Cache<Type, BindResult>();

        public FubuRequest(IRequestData data, IObjectResolver resolver, ILogger logger)
        {
            _logger = logger;
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
            Set(typeof(T), target);
        }

        public void Set(Type type, object target)
        {
            _logger.DebugMessage(() => new SetValueReport{
                Type = type,
                Value = target
            });
            
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

    public class SetValueReport : LogRecord
    {
        public static SetValueReport For<T>(T value)
        {
            return new SetValueReport{
                Type = typeof(T),
                Value = value
            };    
        }

        public SetValueReport(object value)
        {
            Type = value.GetType();
            Value = value;
        }

        public SetValueReport()
        {
        }

        public Type Type { get; set; }
        public object Value { get; set; }

        public bool Equals(SetValueReport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Type, Type) && Equals(other.Value, Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(SetValueReport)) return false;
            return Equals((SetValueReport)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Type: {0}, Value: {1}", Type, Value);
        }
    }
}