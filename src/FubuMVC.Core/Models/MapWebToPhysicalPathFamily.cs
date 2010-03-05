using System;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Models
{
    public class MapWebToPhysicalPathFamily : IConverterFamily
    {
        public bool Matches(PropertyInfo property)
        {
            return property.HasAttribute<MapWebToPhysicalPathAttribute>();
        }

        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            return rawValue =>
            {
                var strVal = rawValue.PropertyValue as String;

                return strVal.IsNotEmpty()
                           ? strVal.PhysicalPath()
                           : strVal;
            };
        }
    }
}