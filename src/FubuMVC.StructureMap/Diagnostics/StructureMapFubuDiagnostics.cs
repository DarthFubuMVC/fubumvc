using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Bottles.Services.Remote;
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

        public SearchResults get_search_Type_Value(StructureMapSearch search)
        {
            var results = new SearchResults
            {
                search = search,
                pluginTypes = new PluginTypeDTO[0],
                instances = new InstanceDTO[0]
            };

            switch (search.Type)
            {
                case "Assembly":
                    var assemblyName = search.Value.Replace(".dll", "");

                    results.pluginTypes =
                        _container.Model.PluginTypes.Where(x => x.PluginType.Assembly.GetName().Name == assemblyName)
                            .Select(x => new PluginTypeDTO(x)).ToArray();

                    break;

                case "Plugin-Type":
                    results.pluginTypes = _container.Model
                        .PluginTypes.Where(x => x.PluginType.GetFullName() == search.Value)
                        .Select(x => new PluginTypeDTO(x))
                        .ToArray();


                    break;

                case "Namespace":
                    results.pluginTypes = _container.Model
                        .PluginTypes.Where(x => x.PluginType.Namespace == search.Value)
                        .Select(x => new PluginTypeDTO(x))
                        .ToArray();
                    break;

                case "Returned-Type":
                    results.instances = instances().Where(x => x.ReturnedType.GetFullName() == search.Value)
                        .Select(x => new InstanceDTO(x)).ToArray();

                    break;

                default:
                    throw new NotImplementedException("DOES NOT COMPUTE");
            }

            return results;
        }

        public string get_whatdoihave()
        {
            return _container.WhatDoIHave();
        }

        public string get_build_plan_PluginType_Name(BuildPlanRequest request)
        {
            var instance = instances().FirstOrDefault(x => x.Name.EqualsIgnoreCase(request.Name) && x.PluginType.GetFullName() == request.PluginType);
            return instance == null
                ? "Cannot find an Instance named '{0}' for PluginType {1}".ToFormat(request.Name, request.PluginType)
                : instance.DescribeBuildPlan();
        }



        public SearchOptions get_summary()
        {
            var options = instances().ToArray();
            var pluginTypes = options.Select(x => x.PluginType).Distinct().ToArray();
            var returnedTypes = options.Select(x => x.ReturnedType).Distinct().Where(x => x != typeof (object)).ToArray();

            var assemblies =
                pluginTypes.Select(x => x.Assembly).Union(returnedTypes.Select(x => x.Assembly)).Distinct().ToArray();

            return new SearchOptions
            {
                assemblies =
                    assemblies.OrderBy(x => x.GetName().Name).Select(assem => new AssemblyDTO(assem, options)).ToArray()
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
            var returnedTypes = options.Select(x => x.ReturnedType).Where(x => x != typeof (object)).ToArray();

            var assemblies =
                pluginTypes.Select(x => x.Assembly).Union(returnedTypes.Select(x => x.Assembly)).Distinct().ToArray();

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
        public InstanceDTO[] instances { get; set; }
        public StructureMapSearch search { get; set; }
    }

    public class StructureMapSearch
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}