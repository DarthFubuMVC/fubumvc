using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;

namespace FubuMVC.Spark.Registration
{
    public class GenericViewModelTypeFinder 
    {
        public Type Parse(string typeName, IEnumerable<Assembly> assemblies)
        {
            if (!IsGeneric(typeName))
                return findType(typeName, assemblies);

            var leftGenericDelimiter = typeName.IndexOf('<');
            var rightGenericDelimiter = typeName.IndexOf('>');
                
            var genericArguments = typeName.Substring(leftGenericDelimiter+1, rightGenericDelimiter - leftGenericDelimiter - 1);
            var genericArgumentsCount = genericArguments.Split(',').Length;
            var openTypeName = typeName.Substring(0, leftGenericDelimiter);
            
            var getTypeFriendlyName = "{0}`{1}[{2}]".ToFormat(openTypeName, genericArgumentsCount, genericArguments);

            return findType(getTypeFriendlyName, assemblies);
        }

        private static Type findType(string typeName, IEnumerable<Assembly> assemblies)
        {
            return assemblies
                .Select(assembly => assembly.GetType(typeName))
                .FirstOrDefault(type => type != null);
        }

        public static bool IsGeneric(string typeName)
        {
            return typeName.IndexOf('<') != -1;
        }
    }
}