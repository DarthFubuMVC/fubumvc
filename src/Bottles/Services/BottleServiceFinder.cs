using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bottles.Diagnostics;
using FubuCore;

namespace Bottles.Services
{
    public static class BottleServiceFinder
    {

        public static IEnumerable<Type> FindTypes(IEnumerable<Assembly> packageAssemblies)
        {
            return packageAssemblies.FilterTypes(BottleService.IsBottleService);
        }

        public static IEnumerable<Type> FilterTypes(this IEnumerable<Assembly> packageAssemblies, Func<Type, bool> predicate)
        {
            var filteredTypes = new List<Type>();

            packageAssemblies.Each(x =>
            {
                try
                {
                    var types = x.GetExportedTypes();
                    filteredTypes.AddRange(types.Where(predicate));
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Unable to find exported types from assembly " + x.FullName, ex);
                }
            });

            return filteredTypes;
        }
    }
}