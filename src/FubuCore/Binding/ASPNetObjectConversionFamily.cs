using System.Reflection;
using FubuCore.Binding;

namespace FubuCore.Binding
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