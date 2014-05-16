using System;
using System.Collections.Generic;
using System.Linq;
using Bottles;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.Registration
{
    public interface IAccessorRulesRegistration
    {
        void AddRules(AccessorRules rules);
    }

    public static class AccessorRulesCompiler
    {
        public static void Compile(BehaviorGraph graph)
        {
            graph.Settings.Replace(() => {
                return PackageRegistry.Timer.Record("Finding AccessorRules", () => {
                    var rules = new AccessorRules();

                    graph.Types()
                        .TypesMatching(
                            x =>
                                x.CanBeCastTo<IAccessorRulesRegistration>() && x.IsConcreteWithDefaultCtor() &&
                                !x.IsOpenGeneric())
                        .
                        Distinct().Select(x => Activator.CreateInstance(x).As<IAccessorRulesRegistration>())
                        .Each(x => x.AddRules(rules));

                    return rules;
                });


            });
        }
    }
}