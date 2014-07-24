using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.Query;

namespace FubuMVC.StructureMap.Diagnostics
{
    public class AssemblyDTO
    {
        public AssemblyDTO(Assembly assembly, IEnumerable<InstanceRef> instances)
        {
            name = assembly.GetName().Name + ".dll";

            var matching = instances.Where(x => x.MatchesAssembly(assembly)).ToArray();
            count = matching.Length;

            namespaces = matching.SelectMany(x => x.Namespaces(assembly))
                .Distinct()
                .OrderBy(x => x)
                .Select(ns => {
                    return new Namespace
                    {
                        name = ns,
                        count = matching.Count(x => x.MatchesNamespace(ns))
                    };
                }).ToArray();
        }

        public AssemblyDTO()
        {
        }

        public string name;
        public int count;
        public Namespace[] namespaces;
    }
}