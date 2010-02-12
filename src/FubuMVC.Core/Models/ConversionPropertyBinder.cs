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
}