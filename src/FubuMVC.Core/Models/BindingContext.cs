using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.Runtime;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.Models
{
    public class BindingContext : IBindingContext
    {
        private static readonly List<Func<PropertyInfo, string>> _namingStrategies;
        protected readonly IServiceLocator _locator;
        protected readonly IRequestData _requestData;

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