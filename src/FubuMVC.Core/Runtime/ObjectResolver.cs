using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Models;

namespace FubuMVC.Core.Runtime
{
    public class ObjectResolver : IObjectResolver
    {
        private readonly List<IModelBinder> _binders = new List<IModelBinder>();

        // Leave this here
        public ObjectResolver()
        {
        }

        public ObjectResolver(IModelBinder[] binders, IValueConverterRegistry converters, ITypeDescriptorRegistry registry)
        {
            _binders.AddRange(binders);
            var defaultBinder = new StandardModelBinder(converters, registry);
            _binders.Add(defaultBinder);
        }

        public virtual BindResult BindModel(Type type, IBindingContext data)
        {
            // TODO:  Throw descriptive error if no binder can be foundow
            // TODO:  Throw descriptive error on a type that cannot be resolved or has errors
            IModelBinder binder = _binders.First(x => x.Matches(type));

            return binder.Bind(type, data);
        }
    }
}