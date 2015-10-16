using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics.Packaging;
using StructureMap.Graph.Scanning;

namespace FubuMVC.Core.Registration
{
    public interface IAccessorRulesRegistration
    {
        void AddRules(AccessorRules rules);
    }

    public static class AccessorRulesCompiler
    {
        public static bool IsAccessorRule(Type type)
        {
            return type.IsConcreteWithDefaultCtor() && type.CanBeCastTo<IAccessorRulesRegistration>();
        }

        public static Task Compile(BehaviorGraph graph, IPerfTimer timer)
        {
            return TypeRepository.FindTypes(graph.AllAssemblies(),
                TypeClassification.Concretes | TypeClassification.Closed, IsAccessorRule)
                .ContinueWith(t =>
                {
                    var rules = new AccessorRules();
                    t.Result.Distinct()                        
                        .Select(x => Activator.CreateInstance(x).As<IAccessorRulesRegistration>())
                        .Each(x => x.AddRules(rules));

                    graph.Settings.Replace(rules);
                });


        }
    }
}