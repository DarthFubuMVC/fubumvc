using System;
using System.Reflection;

namespace FubuMVC.Core.Models
{
    public class ConversionPropertyBinder : IPropertyBinder
    {
        private readonly IValueConverterRegistry _converters;

        public ConversionPropertyBinder(IValueConverterRegistry converters)
        {
            _converters = converters;
        }

        public bool Matches(PropertyInfo property)
        {
            return _converters.FindConverter(property) != null;
        }

        public void Bind(PropertyInfo property, IBindingContext context)
        {
            context.ForProperty(property, () =>
            {
                ValueConverter converter = _converters.FindConverter(property);

                object value = null;
                if (context.PropertyValue != null && property.PropertyType.IsAssignableFrom(context.PropertyValue.GetType()))
                {
                    value = context.PropertyValue;
                }
                else
                {
                    value = converter(context);
                }
                    
                property.SetValue(context.Object, value, null);
            });
        }
    }

    public class NestedObjectPropertyBinder : IPropertyBinder
    {
        public bool Matches(PropertyInfo property)
        {
            return true;
        }

        public void Bind(PropertyInfo property, IBindingContext context)
        {
            context.BindChild(property);
        }
    }
}