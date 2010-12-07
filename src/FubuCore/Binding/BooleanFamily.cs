using System.ComponentModel;
using System.Reflection;

namespace FubuCore.Binding
{
    public class BooleanFamily : IConverterFamily
    {
        public const string Checkbox_On = "on";

        public bool Matches(PropertyInfo property)
        {
            return property.PropertyType.IsTypeOrNullableOf<bool>();
        }

        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            var converter = TypeDescriptor.GetConverter(typeof(bool));

            return x =>
            {
                if (x.PropertyValue is bool) return x.PropertyValue;

                return x.PropertyValue.ToString().Contains(x.Property.Name)
                || x.PropertyValue.ToString().EqualsIgnoreCase(Checkbox_On)
                || (bool) converter.ConvertFrom(x.PropertyValue);
            };
        }
    }
}