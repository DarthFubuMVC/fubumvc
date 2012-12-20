using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.Registration
{
    [ConfigurationType(ConfigurationType.Settings)]
    [Description("Finds and applies property specific rules expressed by classes that implement the IAccessorRulesRegistration interface")]
    public class AccessorOverridesFinder : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var rules = graph.Settings.Get<AccessorRules>();

            TypePool.AppDomainTypes().TypesMatching(x => x.CanBeCastTo<IAccessorRulesRegistration>() && x.IsConcreteWithDefaultCtor() && !x.IsOpenGeneric()).
                  Distinct().Select(x => {
                      return Activator.CreateInstance(x).As<IAccessorRulesRegistration>();
                  })
                 .Each(x => x.AddRules(rules));

            graph.Settings.Replace(rules);
        }
    }
}