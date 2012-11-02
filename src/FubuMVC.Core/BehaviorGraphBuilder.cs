using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Diagnostics;

namespace FubuMVC.Core
{
    public class BehaviorGraphBuilder
    {
        // Need to track the ConfigLog
        public BehaviorGraph Import(FubuRegistry registry, BehaviorGraph parent)
        {
            var graph = BehaviorGraph.ForChild(parent);
            startBehaviorGraph(registry, graph);
            var config = registry.Config;

            config.RunActions(ConfigurationType.Settings, graph);
            config.RunActions(ConfigurationType.Discovery, graph);
            config.RunActions(ConfigurationType.Explicit, graph);
            config.RunActions(ConfigurationType.Policy, graph);
            config.RunActions(ConfigurationType.Reordering, graph);


            return graph;
        }

        public BehaviorGraph Build(FubuRegistry registry)
        {
            var graph = new BehaviorGraph();
            startBehaviorGraph(registry, graph);

            // Need to register the ConfigGraph!!!!

            // use SystemServicesPack
            // use DefaultConfigurationPack

            throw new NotImplementedException();
        }

        private BehaviorGraph startBehaviorGraph(FubuRegistry registry, BehaviorGraph graph)
        {
            var types = registry.BuildTypePool();
            var graph = parent == null ? new BehaviorGraph { Types = types } : new BehaviorGraph(parent){Types = types};

            registry.Config.Add(new DiscoveryActionsConfigurationPack());

            return graph;
        }


    }
}