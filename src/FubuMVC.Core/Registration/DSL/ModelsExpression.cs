using System;
using FubuMVC.Core.Models;

namespace FubuMVC.Core.Registration.DSL
{
    public class ModelsExpression
    {
        private readonly Action<Action<BehaviorGraph>> _callback;

        public ModelsExpression(Action<Action<BehaviorGraph>> callback)
        {
            _callback = callback;
        }

        private ModelsExpression add(Action<BehaviorGraph> configuration)
        {
            _callback(configuration);
            return this;
        }

        public ModelsExpression ConvertUsing<T>() where T : IConverterFamily
        {
            return add(graph => graph.Services.AddService<IConverterFamily, T>());
        }

        public ModelsExpression BindPropertiesWith<T>() where T : IPropertyBinder
        {
            return add(graph => graph.Services.AddService<IPropertyBinder, T>());
        }

        public ModelsExpression BindModelsWith<T>() where T : IModelBinder
        {
            return add(graph => graph.Services.AddService<IModelBinder, T>());
        }
    }
}