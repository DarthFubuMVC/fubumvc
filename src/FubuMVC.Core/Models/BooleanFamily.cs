using System.ComponentModel;
using System.Reflection;
using FubuCore;

namespace FubuMVC.Core.Models
{
    public class BooleanFamily : IConverterFamily
    {
        public bool Matches(PropertyInfo property)
        {
            return property.PropertyType.IsTypeOrNullableOf<bool>();
        }

        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            var converter = TypeDescriptor.GetConverter(typeof(bool));

            return x => 
                   x.PropertyValue.ToString().Contains(x.Property.Name) 
                   || (bool) converter.ConvertFrom(x.PropertyValue);
        }
    }
}