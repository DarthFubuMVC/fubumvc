using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Registration;

namespace FubuMVC.Core.View.Model
{
    public class ViewTypePool
    {
        private readonly TypePool _types;
        private readonly BehaviorGraph _graph;

        public ViewTypePool(BehaviorGraph graph)
        {
            _graph = graph;
            _types = graph.Types();
        }



        public Type FindTypeByName(string typeFullName, Assembly defaultAssembly, Action<string> log)
        {
            if (GenericParser.IsGeneric(typeFullName))
            {
                var genericParser = new GenericParser(_types.Assemblies);
                return genericParser.Parse(typeFullName);
            }

            return findClosedTypeByFullName(typeFullName, defaultAssembly, log);
        }

        private Type findClosedTypeByFullName(string typeFullName, Assembly defaultAssembly, Action<string> log)
        {
            var type = defaultAssembly.GetExportedTypes().FirstOrDefault(x => x.FullName == typeFullName);
            if (type != null)
            {
                return type;
            }

            var types = _types.TypesWithFullName(typeFullName);
            var typeCount = types.Count();

            if (typeCount == 1)
            {
                return types.First();
            }

            log("Unable to set view model type : {0}".ToFormat(typeFullName));

            if (typeCount > 1)
            {
                var candidates = types.Select(x => x.AssemblyQualifiedName).Join(", ");
                log("Type ambiguity on: {0}".ToFormat(candidates));
            }

            return null;
        }

        public Assembly FindAssemblyByProvenance(string provenance)
        {
            if (provenance == ContentFolder.Application) return _graph.ApplicationAssembly;

            return _types.Assemblies.FirstOrDefault(x => x.GetName().Name == provenance) ?? _graph.ApplicationAssembly;
        }
    }
}