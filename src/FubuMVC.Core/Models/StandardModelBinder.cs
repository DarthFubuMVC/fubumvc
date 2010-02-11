using System;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Models
{
    public class StandardModelBinder : IModelBinder
    {
        private readonly IValueConverterRegistry _converters;
        private readonly ITypeDescriptorRegistry _typeRegistry;

        public StandardModelBinder(IValueConverterRegistry converters, ITypeDescriptorRegistry typeRegistry)
        {
            _converters = converters;
            _typeRegistry = typeRegistry;
        }

        public bool Matches(Type type)
        {
            return type.GetConstructors().Count(x => x.GetParameters().Length == 0) == 1;
        }

        public object Bind(Type type, IBindingContext context)
        {
            object instance = Activator.CreateInstance(type);
            Bind(type, instance, context);
            
            return instance;
        }

        public void Bind(Type type, object instance, IBindingContext context)
        {
            context.StartObject(instance);
            populate(type, context);
            context.FinishObject();
        }


        // Only exists for easier testing
        public void Populate(object target, IBindingContext context)
        {
            context.StartObject(target);
            populate(target.GetType(), context);
            context.FinishObject();
        }

        private void populate(Type type, IBindingContext context)
        {
            _typeRegistry.ForEachProperty(type, prop =>
            {
                context.ForProperty(prop, () =>
                {
                    var converter = _converters.FindConverter(prop);
                    object value = converter(context);
                    prop.SetValue(context.Object, value, null);
                });
            });
        }

    }
}