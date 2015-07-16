using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics.Packaging;

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

        public static void Compile(BehaviorGraph graph, IPerfTimer timer)
        {
            graph.Settings.Replace(() => {
                return timer.Record("Finding AccessorRules", () => {
                    var rules = new AccessorRules();

                    TypeRepository.FindTypes(graph.AllAssemblies(), TypeClassification.Concretes | TypeClassification.Closed, IsAccessorRule).Result().Distinct()
                        .Select(x => Activator.CreateInstance(x).As<IAccessorRulesRegistration>())
                        .Each(x => x.AddRules(rules));

                    return rules;
                });


            });
        }
    }
}