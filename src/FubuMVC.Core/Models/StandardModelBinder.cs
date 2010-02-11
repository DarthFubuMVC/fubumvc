using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FubuMVC.Core.Models
{
    public interface IPropertyBinder
    {
        bool Matches(PropertyInfo property);
        void Bind(PropertyInfo property, IBindingContext context);
    }

    public class ConversionPropertyBinder : IPropertyBinder
    {
        private readonly IValueConverterRegistry _converters;

        public ConversionPropertyBinder(IValueConverterRegistry converters)
        {
            _converters = converters;
        }

        public bool Matches(PropertyInfo property)
        {
            // TODO -- make this filter on whether or not it can find a converter
            return true;
        }

        public void Bind(PropertyInfo property, IBindingContext context)
        {
            context.ForProperty(property, () =>
            {
                ValueConverter converter = _converters.FindConverter(property);
                object value = converter(context);
                property.SetValue(context.Object, value, null);
            });
        }
    }

    public class PropertyBinderRegistry
    {
        private readonly IList<IPropertyBinder> _binders = new List<IPropertyBinder>();

        public PropertyBinderRegistry(IEnumerable<IPropertyBinder> binders)
        {
            _binders.AddRange(binders);
        }
    }


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
                    ValueConverter converter = _converters.FindConverter(prop);
                    object value = converter(context);
                    prop.SetValue(context.Object, value, null);
                });
            });
        }
    }
}