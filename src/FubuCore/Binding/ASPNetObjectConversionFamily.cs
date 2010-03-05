using System.Reflection;

namespace FubuMVC.Core.Models
{
    public class ASPNetObjectConversionFamily : IConverterFamily
    {
        public bool Matches(PropertyInfo property)
        {
            return AggregateDictionary.IsSystemProperty(property);
        }

        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            return context => context.PropertyValue;
        }
    }
}