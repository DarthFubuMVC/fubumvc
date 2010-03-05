using System;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Util;

namespace FubuCore.Binding
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