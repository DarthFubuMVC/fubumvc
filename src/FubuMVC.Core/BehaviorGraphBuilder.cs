using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Diagnostics;
using System.Collections.Generic;

namespace FubuMVC.Core
{
    public static class BehaviorGraphBuilder
    {
        // Need to track the ConfigLog
        public static BehaviorGraph Import(FubuRegistry registry, BehaviorGraph parent)
        {
            var graph = BehaviorGraph.ForChild(parent);
            startBehaviorGraph(registry, graph);
            var config = registry.Config;

            config.RunActions(ConfigurationType.Settings, graph);
            config.RunActions(ConfigurationType.Discovery, graph);

            config.UniqueImports().Each(import => import.ImportInto(graph));

            config.RunActions(ConfigurationType.Explicit, graph);
            config.RunActions(ConfigurationType.Policy, graph);
            config.RunActions(ConfigurationType.Reordering, graph);


            return graph;
        }

        public static BehaviorGraph Build(FubuRegistry registry)
        {
            var graph = new BehaviorGraph();
            startBehaviorGraph(registry, graph);
            var config = registry.Config;

            config.AllServiceRegistrations().Each(x => {
                
            });


            config.Add(new SystemServicesPack());
            config.Add(new DefaultConfigurationPack());

            config.RunActions(ConfigurationType.Settings, graph);
            config.RunActions(ConfigurationType.Discovery, graph);

            config.UniqueImports().Each(import => import.ImportInto(graph));

            config.RunActions(ConfigurationType.Explicit, graph);
            config.RunActions(ConfigurationType.Policy, graph);
            config.RunActions(ConfigurationType.Navigation, graph);
            config.RunActions(ConfigurationType.ByNavigation, graph);
            config.RunActions(ConfigurationType.Attributes, graph);
            config.RunActions(ConfigurationType.ModifyRoutes, graph);
            config.RunActions(ConfigurationType.InjectNodes, graph);
            config.RunActions(ConfigurationType.Conneg, graph);
            config.RunActions(ConfigurationType.Attachment, graph);
            config.RunActions(ConfigurationType.Reordering, graph);
            config.RunActions(ConfigurationType.Instrumentation, graph);


            graph.Services.AddService(config);

            return graph;
        }

        private static void startBehaviorGraph(FubuRegistry registry, BehaviorGraph graph)
        {
            var types = registry.BuildTypePool();
            graph.Types = types;
            registry.Config.Add(new DiscoveryActionsConfigurationPack());
        }


    }
}