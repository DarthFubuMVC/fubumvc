using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Spark.SparkModel.Utils
{
    public static class PrettyTypeNamesExtensions
    {
        public static string PrettyFullName(this Type type)
        {
            if (type.IsGenericType)
            {
                var innerTypes = type
                    .GetGenericArguments()
                    .Select(t => t.PrettyFullName())
                    .ToArray()
                    .Join(",");

                var name = type.Name.Split('`').First();
                return string.Format("{0}.{1}[[{2}]]", type.Namespace, name, innerTypes);
            }
            return type.FullName;
        }
    }
}