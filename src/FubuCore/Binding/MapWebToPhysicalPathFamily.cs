using System;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;

namespace FubuCore.Binding
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