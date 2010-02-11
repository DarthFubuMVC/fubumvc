using System;
using System.Reflection;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Models
{
    public class MapFromWebPathFamily : IConverterFamily
    {
        public bool Matches(PropertyInfo property)
        {
            return property.HasAttribute<MapFromWebPathAttribute>();
        }

        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            return rawValue =>
            {
                var strVal = rawValue.PropertyValue as String;

                return strVal.IsNotEmpty()
                           ? strVal.MapPath()
                           : strVal;
            };
        }
    }
}