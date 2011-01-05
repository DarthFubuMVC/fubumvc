using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using FubuCore.Reflection;

namespace FubuCore.CommandLine
{
    public static class InputParser
    {
        public static readonly string FLAG_PREFIX = "-";
        public static readonly string FLAG_SUFFIX = "Flag";
        private static readonly ObjectConverter _converter = new ObjectConverter();


        public static List<ITokenHandler> GetHandlers(Type inputType)
        {
            return inputType.GetProperties()
                .Where(prop => prop.CanWrite)
                .Where(prop => !prop.HasAttribute<IgnoreOnCommandLineAttribute>())
                .Select(BuildHandler).ToList();
        }

        public static ITokenHandler BuildHandler(PropertyInfo property)
        {
            if (!property.Name.EndsWith(FLAG_SUFFIX))
            {
                return new Argument(property, _converter);
            }

            if (property.PropertyType == typeof(bool))
            {
                return new BooleanFlag(property);
            }

            return new Flag(property, _converter);
        }

        public static string ToFlagName(PropertyInfo property)
        {
            var name = property.Name.Substring(0, property.Name.Length - 4);
            property.ForAttribute<FlagAliasAttribute>(att => name = att.Alias);

            return FLAG_PREFIX + name.ToLower();
        }

    }
}