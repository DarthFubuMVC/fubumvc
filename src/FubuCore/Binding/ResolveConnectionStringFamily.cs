using System;
using System.Configuration;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;

namespace FubuCore.Binding
{
    public class ResolveConnectionStringFamily : IConverterFamily
    {
        public bool Matches(PropertyInfo property)
        {
            return property.HasAttribute<ConnectionStringAttribute>();
        }

        public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo property)
        {
            return rawValue =>
            {
                var strVal = rawValue.PropertyValue as String;

                return strVal.IsNotEmpty()
                           ? getConnectionString(strVal)
                           : strVal;
            };
        }

        public static Func<string, ConnectionStringSettings> GetConnectionStringSettings = key => ConfigurationManager.ConnectionStrings[key];

        private static string getConnectionString(string name)
        {
            var connectionStringSettings = GetConnectionStringSettings(name);
            return connectionStringSettings != null
                ? connectionStringSettings.ConnectionString
                : name;
        }
    }
}