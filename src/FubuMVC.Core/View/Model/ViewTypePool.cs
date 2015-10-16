using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View.Registration;
using StructureMap.Graph.Scanning;

namespace FubuMVC.Core.View.Model
{
    public class ViewTypePool
    {
        private readonly BehaviorGraph _graph;

        public ViewTypePool(BehaviorGraph graph)
        {
            _graph = graph;
        }


        public Type FindTypeByName(string typeFullName, Action<string> log)
        {
            if (GenericParser.IsGeneric(typeFullName))
            {
                var genericParser = new GenericParser(_graph.AllAssemblies());
                return genericParser.Parse(typeFullName);
            }

            return findClosedTypeByFullName(typeFullName, log);
        }

        private Type findClosedTypeByFullName(string typeFullName, Action<string> log)
        {


            var types =
                TypeRepository.FindTypes(_graph.AllAssemblies(),
                    TypeClassification.Closed | TypeClassification.Concretes, x => x.FullName == typeFullName).Result();

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
    }
}