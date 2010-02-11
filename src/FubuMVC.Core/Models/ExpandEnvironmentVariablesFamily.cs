using System;
using System.Reflection;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Models
{
    public class ExpandEnvironmentVariablesFamily : IConverterFamily
    {
        public bool Matches(PropertyInfo property)
        {
            return property.HasAttribute<ExpandEnvironmentVariablesAttribute>();
        }

        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            return rawValue =>
                   {
                       var strVal = rawValue.PropertyValue as String;

                       return strVal.IsNotEmpty()
                                  ? Environment.ExpandEnvironmentVariables(strVal)
                                  : strVal;
                   };
        }
    }
}