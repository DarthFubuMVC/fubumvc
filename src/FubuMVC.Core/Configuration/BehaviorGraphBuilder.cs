using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core.Configuration
{
    internal static class BehaviorGraphBuilder
    {
        public static BehaviorGraph Import(FubuRegistry registry, SettingsCollection parentSettings)
        {
            var graph = new BehaviorGraph(parentSettings);
            startBehaviorGraph(registry, graph);
            var config = registry.Config;

            config.RunActions(ConfigurationType.Settings, graph);

            config.Sources.Union(config.Imports)
                .SelectMany(x => x.BuildChains(graph.Settings))
                .Each(chain => graph.AddChain(chain));

            config.RunActions(ConfigurationType.Explicit, graph);
            config.RunActions(ConfigurationType.Policy, graph);
            config.RunActions(ConfigurationType.Reordering, graph);
            config.RunActions(ConfigurationType.InjectNodes, graph);

            return graph;
        }

        public static BehaviorGraph Build(FubuRegistry registry)
        {
            var graph = new BehaviorGraph();
            startBehaviorGraph(registry, graph);

            var config = registry.Config;

            // Apply settings
            config.RunActions(ConfigurationType.Settings, graph);

            var viewDiscovery = graph.Settings.Get<ViewEngines>().BuildViewBag(graph);
            lookForAccessorOverrides(graph);

            config.Add(new SystemServicesPack());
            config.Add(new DefaultConfigurationPack());

            discoverChains(config, graph);
            var attacher = new ViewAttachmentWorker(viewDiscovery.Result, graph.Settings.Get<ViewAttachmentPolicy>());
            attacher.Configure(graph);

            config.RunActions(ConfigurationType.Explicit, graph);
            config.RunActions(ConfigurationType.Policy, graph);
            config.RunActions(ConfigurationType.InjectNodes, graph);
            config.RunActions(ConfigurationType.Attachment, graph);

            // apply the authorization, input, and output nodes
            graph.Behaviors.Each(x => x.InsertNodes(graph.Settings.Get<ConnegSettings>()));

            config.RunActions(ConfigurationType.Reordering, graph);
            config.RunActions(ConfigurationType.Instrumentation, graph);

            registerServices(config, graph);


            return graph;
        }

        private static void discoverChains(ConfigGraph config, BehaviorGraph graph)
        {
            var chainSources = config.Sources.Union(config.UniqueImports()).ToList();
            if (FubuMode.InDevelopment())
            {
                var aggregator = new ActionSourceAggregator(null);
                aggregator.Add(new RegisterAbout());

                chainSources.Add(aggregator);
            }

            var tasks =
                chainSources.Select(
                    x => {
                        return
                            Task.Factory.StartNew(
                                () => { x.BuildChains(graph.Settings).Each(chain => graph.AddChain(chain)); });
                    }).ToArray();

            Task.WaitAll(tasks);
        }

        private static void registerServices(ConfigGraph config, BehaviorGraph graph)
        {
            graph.Settings.Register(graph.Services);

            config
                .AllServiceRegistrations()
                .OfType<IServiceRegistration>()
                .Each(x => x.Apply(graph.Services));

            graph.Services.AddService(config);
        }


        // TODO -- try to eliminate the duplication from above
        public static IEnumerable<string> ConfigurationOrder()
        {
            return new string[]
            {
                ConfigurationType.Settings,
                ConfigurationType.Explicit,
                ConfigurationType.Policy,
                ConfigurationType.InjectNodes,
                ConfigurationType.Attachment,
                ConfigurationType.Reordering,
                ConfigurationType.Instrumentation
            };
        }


        private static void startBehaviorGraph(FubuRegistry registry, BehaviorGraph graph)
        {
            graph.ApplicationAssembly = registry.ApplicationAssembly;


            findAutoRegisteredConfigurationActions(registry, graph);
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
                        x =>
                            x.CanBeCastTo<IAccessorRulesRegistration>() && x.IsConcreteWithDefaultCtor() &&
                            !x.IsOpenGeneric())
                    .
                    Distinct().Select(x => { return Activator.CreateInstance(x).As<IAccessorRulesRegistration>(); })
                    .Each(x => x.AddRules(rules));

                return rules;
            });
        }
    }
}