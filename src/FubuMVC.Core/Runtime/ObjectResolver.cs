using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Models;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.Runtime
{
    public class ObjectResolver : IObjectResolver
    {
        private readonly IServiceLocator _services;
        private readonly List<IModelBinder> _binders = new List<IModelBinder>();

        // Leave this here
        public ObjectResolver()
        {
        }

        public ObjectResolver(IModelBinder[] binders, IValueConverterRegistry converters, ITypeDescriptorRegistry registry, IServiceLocator services)
        {
            _services = services;
            _binders.AddRange(binders);
            var defaultBinder = new StandardModelBinder(converters, registry);
            _binders.Add(defaultBinder);
        }

        public virtual BindResult BindModel(Type type, IRequestData data)
        {
            var context = new BindingContext(data, _services);
            return BindModel(type, context);
        }

        public BindResult BindModel(Type type, IBindingContext context)
        {
            // TODO:  Throw descriptive error if no binder can be foundow
            // TODO:  Throw descriptive error on a type that cannot be resolved or has errors

            IModelBinder binder = _binders.First(x => x.Matches(type));
            return new BindResult()
            {
                Value = binder.Bind(type, context),
                Problems = context.Problems
            };
        }
    }
}