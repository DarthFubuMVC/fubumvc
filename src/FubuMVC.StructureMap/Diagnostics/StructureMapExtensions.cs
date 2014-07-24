using System.Collections.Generic;
using System.Reflection;
using StructureMap.Query;

namespace FubuMVC.StructureMap.Diagnostics
{
    public static class StructureMapExtensions
    {
        public static bool MatchesAssembly(this InstanceRef instance, Assembly assembly)
        {
            return instance.ReturnedType.Assembly == assembly || instance.PluginType.Assembly == assembly;
        }

        public static bool MatchesNamespace(this InstanceRef instance, string ns)
        {
            return instance.ReturnedType.Namespace == ns || instance.PluginType.Namespace == ns;
        }

        public static IEnumerable<string> Namespaces(this InstanceRef instance, Assembly assembly)
        {
            if (instance.PluginType.Assembly == assembly)
            {
                yield return instance.PluginType.Namespace;
            }

            if (instance.ReturnedType.Assembly == assembly)
            {
                yield return instance.ReturnedType.Namespace;
            }
        } 
    }
}