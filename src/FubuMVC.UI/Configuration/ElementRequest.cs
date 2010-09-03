using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.UI.Configuration
{
    public class ElementRequest
    {
        private readonly Stringifier _stringifier;
        private bool _hasFetched;
        private object _rawValue;
        private readonly IServiceLocator _services;

        public static ElementRequest For<T>(T model, Expression<Func<T, object>> expression)
        {
            return new ElementRequest(model, expression.ToAccessor(), null, null);
        }

        public static ElementRequest For<T>(T model, Expression<Func<T, object>> expression, IServiceLocator services)
        {
            return new ElementRequest(model, expression.ToAccessor(), services, null);
        }

        public ElementRequest(object model, Accessor accessor, IServiceLocator services, Stringifier stringifier)
        {
            _stringifier = stringifier;
            Model = model;
            Accessor = accessor;
            _services = services;
        }

        public string ElementId { get; set; }


        public object RawValue
        {
            get
            {
                if (!_hasFetched)
                {
                    _rawValue = Accessor.GetValue(Model);
                    _hasFetched = true;
                }

                return _rawValue;
            }
        }

        public object Model { get; private set; }
        public Accessor Accessor { get; private set; }

        public AccessorDef ToAccessorDef()
        {
            return new AccessorDef
            {
                Accessor = Accessor,
                ModelType = HolderType()
            };
        }

        
        public Type HolderType()
        {
            if (Model == null)
            {
                return Accessor.DeclaringType;
            }

            var resolver = _services == null ? new TypeResolver() : Get<ITypeResolver>();

            return Model == null ? null : resolver.ResolveType(Model);
        }

        public T Get<T>()
        {
            return _services.GetInstance<T>();
        }

        public T Value<T>()
        {
            return (T) RawValue;
        }

        // TODO -- unit tests around this method
        public string StringValue()
        {
            var request = new GetStringRequest(Accessor, RawValue, _services);
            return _stringifier.GetString(request);
        }

        public bool ValueIsEmpty()
        {
            return RawValue == null || string.Empty.Equals(RawValue);
        }

        public void ForValue<T>(Action<T> action)
        {
            if (ValueIsEmpty()) return;

            action((T) RawValue);
        }
    }
}