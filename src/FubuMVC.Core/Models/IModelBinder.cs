using System;
using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Runtime;
using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace FubuMVC.Core.Models
{
    public interface IModelBinder
    {
        bool Matches(Type type);
        BindResult Bind(Type type, object instance, IRequestData data);
        BindResult Bind(Type type, IRequestData data);
    }

    public interface IBindingContext
    {
        T Service<T>();
        void Value(PropertyInfo property, Action<object> callback);
        IBindingContext PrefixWith(string prefix);
    }

    public class BindingContext : IBindingContext
    {
        private readonly IRequestData _requestData;
        private readonly IServiceLocator _locator;
        private static readonly List<Func<PropertyInfo, string>> _namingStrategies;

        static BindingContext()
        {
            _namingStrategies = new List<Func<PropertyInfo, string>>
            {
                p => p.Name,
                p => p.Name.Replace("_", "-")
            };
        }

        public BindingContext(IRequestData requestData, IServiceLocator locator)
        {
            _requestData = requestData;
            _locator = locator;
        }

        public T Service<T>()
        {
            return _locator.GetInstance<T>();
        }

        public void Value(PropertyInfo property, Action<object> callback)
        {
            _namingStrategies.Any(naming =>
            {
                string name = naming(property);
                return _requestData.Value(name, callback);
            });
        }

        public IBindingContext PrefixWith(string prefix)
        {
            var prefixed = new PrefixedRequestData(_requestData, prefix);
            return new BindingContext(prefixed, _locator);
        }
    }
}