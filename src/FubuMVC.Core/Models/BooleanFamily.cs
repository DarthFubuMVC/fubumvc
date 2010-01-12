using System.Reflection;

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
            return x => 
                   x.Value.ToString().Contains(x.Property.Name) 
                   || (bool) ValueConverterRegistry.BasicConvert(typeof (bool), x.Value);
        }
    }
}