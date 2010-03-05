using System;
using System.Reflection;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Models
{
    public class ConversionPropertyBinder : IPropertyBinder
    {
        private readonly Cache<PropertyInfo, ValueConverter> _cache = new Cache<PropertyInfo, ValueConverter>();

        public ConversionPropertyBinder(IValueConverterRegistry converters)
        {
            _cache.OnMissing = prop => converters.FindConverter(prop);
        }

        public bool Matches(PropertyInfo property)
        {
            return _cache[property] != null;
        }

        // TODO -- need an integrated test with Connection String providers
        public void Bind(PropertyInfo property, IBindingContext context)
        {
            context.ForProperty(property, () =>
            {
                ValueConverter converter = _cache[property];

                var value = converter(context);
                    
                property.SetValue(context.Object, value, null);
            });
        }
    }
}