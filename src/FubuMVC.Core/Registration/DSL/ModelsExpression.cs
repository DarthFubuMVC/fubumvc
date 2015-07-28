using System;
using System.Reflection;
using FubuCore.Binding;

namespace FubuMVC.Core.Registration.DSL
{
    public class ModelsExpression
    {
        private readonly ServiceRegistry _registry;

        public ModelsExpression(ServiceRegistry registry)
        {
            _registry = registry;
        }

        public ModelsExpression ConvertUsing<T>() where T : IConverterFamily
        {
            _registry.AddService<IConverterFamily, T>();
            return this;
        }

        public ModelsExpression BindPropertiesWith<T>() where T : IPropertyBinder
        {
            _registry.AddService<IPropertyBinder, T>();
            return this;
        }

        public ModelsExpression BindModelsWith<T>() where T : IModelBinder
        {
            _registry.AddService<IModelBinder, T>();
            return this;
        }

        public ModelsExpression IgnoreProperties(Func<PropertyInfo, bool> filter)
        {
            var binder = new IgnorePropertyBinder(filter);
            _registry.AddService<IPropertyBinder>(binder);

            return this;
        }
    }
}