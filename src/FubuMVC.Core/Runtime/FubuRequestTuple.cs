using System;
using System.ComponentModel;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuCore;

namespace FubuMVC.Core.Runtime
{
    /// <summary>
    /// Marker interface.  Directs the model binding to find every property
    /// by just executing IFubuRequest
    /// </summary>
    public interface FubuRequestTuple
    {
        
    }

    [Description("For models that implement the FubuRequestTuple marker interface")]
    public class FubuTupleBinder : IModelBinder
    {
        private readonly ITypeDescriptorCache _types;

        public FubuTupleBinder(ITypeDescriptorCache types)
        {
            _types = types;
        }

        public bool Matches(Type type)
        {
            return type.CanBeCastTo<FubuRequestTuple>();
        }

        public void Bind(Type type, object instance, IBindingContext context)
        {
            var request = context.Service<IFubuRequest>();

            _types.ForEachProperty(type, prop =>
            {
                var value = request.Get(prop.PropertyType);
                prop.SetValue(instance, value, null);
            });
        }

        public object Bind(Type type, IBindingContext context)
        {
            var model = Activator.CreateInstance(type);
            Bind(type, model, context);

            return model;
        }
    }
}