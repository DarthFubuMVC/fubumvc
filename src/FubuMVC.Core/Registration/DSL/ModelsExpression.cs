using System;
using System.Reflection;
using FubuCore.Binding;

namespace FubuMVC.Core.Registration.DSL
{
    public class ModelsExpression
    {
        private readonly Action<Action<IServiceRegistry>> _callback;

        public ModelsExpression(Action<Action<IServiceRegistry>> callback)
        {
            _callback = callback;
        }

        private ModelsExpression add(Action<IServiceRegistry> configuration)
        {
            _callback(configuration);
            return this;
        }

        public ModelsExpression ConvertUsing<T>() where T : IConverterFamily
        {
            return add(x => x.AddService<IConverterFamily, T>());
        }

        public ModelsExpression BindPropertiesWith<T>() where T : IPropertyBinder
        {
            return add(x => x.AddService<IPropertyBinder, T>());
        }

        public ModelsExpression BindModelsWith<T>() where T : IModelBinder
        {
            return add(x => x.AddService<IModelBinder, T>());
        }

        public ModelsExpression IgnoreProperties(Func<PropertyInfo, bool> filter)
        {
            var binder = new IgnorePropertyBinder(filter);
            return add(x => x.AddService<IPropertyBinder>(binder));
        }
    }
}