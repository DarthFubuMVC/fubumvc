using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime;
using StructureMap;
using StructureMap.Query;

namespace FubuMVC.StructureMap.Diagnostics
{
    public class StructureMapFubuDiagnostics
    {
        private readonly IContainer _container;

        public StructureMapFubuDiagnostics(IServiceFactory facility)
        {
            if (facility is StructureMapContainerFacility)
            {
                _container = facility.As<StructureMapContainerFacility>().Container;
            }
            else
            {
                _container = new Container();
            }

        }

        private IEnumerable<InstanceRef> instances()
        {
            foreach (var pluginType in _container.Model.PluginTypes)
            {
                foreach (var instanceRef in pluginType.Instances)
                {
                    yield return instanceRef;
                }

                if (pluginType.MissingNamedInstance != null) yield return pluginType.MissingNamedInstance;

                if (pluginType.Fallback != null) yield return pluginType.Fallback;
            }
        }

        public string get_search_Type_Value(StructureMapSearch search)
        {
            return "Type: {0}, Value: {1}".ToFormat(search.Type, search.Value);
        }

        public SearchResults get_build_plan__PluginType_Id(BuildPlanRequest request)
        {
            var results = new SearchResults
            {
                pluginTypes = new PluginTypeDTO[0]
            };


            return results;
        }

        public SearchOptions get_summary()
        {
            var options = instances().ToArray();
            var pluginTypes = options.Select(x => x.PluginType).Distinct().ToArray();
            var returnedTypes = options.Select(x => x.ReturnedType).Where(x => x != typeof (object)).ToArray();

            var assemblies = pluginTypes.Select(x => x.Assembly).Union(returnedTypes.Select(x => x.Assembly)).Distinct().ToArray();

            return new SearchOptions
            {
                assemblies = assemblies.OrderBy(x => x.GetName().Name).Select(assem => new AssemblyDTO(assem, options)).ToArray()
            };
        }

        public SearchOption[] get_search_options()
        {
            return findOptions().ToArray();
        }

        private IEnumerable<SearchOption> findOptions()
        {
            var options = instances().ToArray();
            var pluginTypes = options.Select(x => x.PluginType).Distinct().ToArray();
            var returnedTypes = options.Select(x => x.ReturnedType).Where(x => x != typeof(object)).ToArray();

            var assemblies = pluginTypes.Select(x => x.Assembly).Union(returnedTypes.Select(x => x.Assembly)).Distinct().ToArray();

            foreach (var assembly in assemblies)
            {
                yield return new SearchOption(assembly);
            }

            foreach (var pluginType in pluginTypes)
            {
                yield return SearchOption.ForPluginType(pluginType);
            }

            foreach (var returnedType in returnedTypes)
            {
                yield return SearchOption.ForReturnedType(returnedType);
            }

            var namespaces = pluginTypes.Union(returnedTypes).Select(x => x.Namespace).Distinct();
            foreach (var ns in namespaces)
            {
                yield return SearchOption.ForNamespace(ns);
            }
        } 

    }

    public class SearchResults
    {
        public PluginTypeDTO[] pluginTypes { get; set; }
    }

    public class StructureMapSearch
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}