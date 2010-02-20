using System.Reflection;

namespace FubuMVC.Core.Models
{
    public class PassthroughConverter<T> : IConverterFamily
    {
        public bool Matches(PropertyInfo property)
        {
            return property.PropertyType.IsAssignableFrom(typeof (T));
        }

        // Like it says, straight pass through
        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            return context => context.PropertyValue;
        }
    }
}