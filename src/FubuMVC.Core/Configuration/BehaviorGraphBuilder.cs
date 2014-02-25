using System;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using System.Collections.Generic;

namespace FubuMVC.Core.Configuration
{
    internal static class BehaviorGraphBuilder
    {
        // Need to track the ConfigLog
        public static BehaviorGraph Import(FubuRegistry registry, BehaviorGraph parent)
        {
            var graph = BehaviorGraph.ForChild(parent);
            startBehaviorGraph(registry, graph);
            var config = registry.Config;

            config.RunActions(ConfigurationType.Settings, graph);

            config.RunActions(ConfigurationType.Discovery, graph);

            config.Imports.Each(import => import.ImportInto(graph));

            config.RunActions(ConfigurationType.Explicit, graph);
            config.RunActions(ConfigurationType.Policy, graph);
            config.RunActions(ConfigurationType.Reordering, graph);
            config.RunActions(ConfigurationType.Attributes, graph);
            config.RunActions(ConfigurationType.ModifyRoutes, graph);
            config.RunActions(ConfigurationType.InjectNodes, graph);
            config.RunActions(ConfigurationType.Conneg, graph);

            return graph;
        }

        public static BehaviorGraph Build(FubuRegistry registry)
        {
            var graph = new BehaviorGraph();
            startBehaviorGraph(registry, graph);
            var config = registry.Config;

            config.Add(new SystemServicesPack());
            config.Add(new DefaultConfigurationPack());

            // Apply settings
            config.RunActions(ConfigurationType.Settings, graph);
            config.Add(new RegisterAllSettings(graph));

            config
                .AllServiceRegistrations()
                .OfType<IServiceRegistration>()
                .Each(x => x.Apply(graph.Services));

            
            config.RunActions(ConfigurationType.Discovery, graph);

            config.UniqueImports().Each(import => import.ImportInto(graph));

            config.RunActions(ConfigurationType.Explicit, graph);
            config.RunActions(ConfigurationType.Policy, graph);
            config.RunActions(ConfigurationType.Attributes, graph);
            config.RunActions(ConfigurationType.ModifyRoutes, graph);
            config.RunActions(ConfigurationType.InjectNodes, graph);
            config.RunActions(ConfigurationType.Conneg, graph);
            config.RunActions(ConfigurationType.Attachment, graph);

            // apply the authorization, input, and output nodes
            graph.Behaviors.Each(x => x.InsertNodes());

            config.RunActions(ConfigurationType.Navigation, graph);
            config.RunActions(ConfigurationType.ByNavigation, graph);
            config.RunActions(ConfigurationType.Reordering, graph);
            config.RunActions(ConfigurationType.Instrumentation, graph);


            graph.Services.AddService(config);

            return graph;
        }


        // TODO -- try to eliminate the duplication from above
        public static IEnumerable<string> ConfigurationOrder()
        {
            return new string[]
                   {
                       ConfigurationType.Settings,
                       ConfigurationType.Discovery,
                       ConfigurationType.Explicit,
                       ConfigurationType.Policy,
                       ConfigurationType.Attributes,
                       ConfigurationType.ModifyRoutes,
                       ConfigurationType.InjectNodes,
                       ConfigurationType.Conneg,
                       ConfigurationType.Attachment,
                       ConfigurationType.Navigation,
                       ConfigurationType.ByNavigation,
                       ConfigurationType.Reordering,
                       ConfigurationType.Instrumentation
                   };
        } 

        

        private static void startBehaviorGraph(FubuRegistry registry, BehaviorGraph graph)
        {
            graph.ApplicationAssembly = registry.ApplicationAssembly;

            lookForAccessorOverrides(graph);

            findAutoRegisteredConfigurationActions(registry, graph);

            registry.Config.Add(new DiscoveryActionsConfigurationPack());
        }

        private static void findAutoRegisteredConfigurationActions(FubuRegistry registry, BehaviorGraph graph)
        {
            var types =
                graph.ApplicationAssembly.GetExportedTypes()
                    .Where(x => x.HasAttribute<AutoImportAttribute>() && x.IsConcreteWithDefaultCtor())
                    .ToArray();
            types.Where(x => x.CanBeCastTo<IFubuRegistryExtension>())
                .Each(x => { Activator.CreateInstance(x).As<IFubuRegistryExtension>().Configure(registry); });

            types.Where(x => x.CanBeCastTo<IConfigurationAction>()).Each(x => {
                var policy = Activator.CreateInstance(x).As<IConfigurationAction>();
                registry.Policies.Add(policy);
            });
        }

        private static void lookForAccessorOverrides(BehaviorGraph graph)
        {
            graph.Settings.Replace(() => {
                var rules = new AccessorRules();

                TypePool.AppDomainTypes()
                    .TypesMatching(
                        x => x.CanBeCastTo<IAccessorRulesRegistration>() && x.IsConcreteWithDefaultCtor() && !x.IsOpenGeneric())
                    .
                    Distinct().Select(x => { return Activator.CreateInstance(x).As<IAccessorRulesRegistration>(); })
                    .Each(x => x.AddRules(rules));

                return rules;
            });
        }
    }
}