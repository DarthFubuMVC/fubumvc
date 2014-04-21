using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;
using HtmlTags.Conventions;

namespace FubuMVC.Core.Configuration
{
    internal static class BehaviorGraphBuilder
    {
        public static BehaviorGraph Import(FubuRegistry registry, BehaviorGraph parentGraph)
        {
            var graph = new BehaviorGraph(parentGraph.Settings);
            startBehaviorGraph(registry, graph);
            var config = registry.Config;

            config.RunActions(ConfigurationType.Settings, graph);

            config.Sources.Union(config.Imports)
                .SelectMany(x => x.BuildChains(graph))
                .Each(chain => graph.AddChain(chain));

            config.RunActions(ConfigurationType.Explicit, graph);
            config.RunActions(ConfigurationType.Policy, graph);
            config.RunActions(ConfigurationType.Reordering, graph);

            return graph;
        }

        // TOOD -- clean this up a little bit
        public static BehaviorGraph Build(FubuRegistry registry)
        {
            var graph = new BehaviorGraph();
            startBehaviorGraph(registry, graph);

            var config = registry.Config;
            
            // TODO -- settings from the application must always win

            // Apply settings
            config.RunActions(ConfigurationType.Settings, graph);

            graph.Settings.Alter<ConnegSettings>(x => x.Graph = ConnegGraph.Build(graph));

            var assetDiscovery = graph.Settings.Get<AssetSettings>().Build(graph.Files)
                .ContinueWith(t => graph.Services.AddService<IAssetGraph>(t.Result));

            var viewDiscovery = graph.Settings.Get<ViewEngineSettings>().BuildViewBag(graph);
            var layoutAttachmentTasks =
                viewDiscovery.ContinueWith(
                    t => graph.Settings.Get<ViewEngineSettings>().Facilities.Select(x => x.LayoutAttachment).ToArray());

            graph.Settings.Replace(viewDiscovery);

            lookForAccessorOverrides(graph);
            var htmlConventionCollation = graph.Settings.GetTask<AccessorRules>().ContinueWith(t => {
                var library = graph.Settings.Get<HtmlConventionLibrary>();
                HtmlConventionCollator.BuildHtmlConventionLibrary(library, t.Result);

                graph.Services.Clear(typeof(HtmlConventionLibrary));
                graph.Services.AddService(library);
            });

            config.Add(new SystemServicesPack());
            config.Add(new DefaultConfigurationPack());

            discoverChains(config, graph);

            viewDiscovery.Wait(5000);
            var attacher = new ViewAttachmentWorker(viewDiscovery.Result, graph.Settings.Get<ViewAttachmentPolicy>());
            attacher.Configure(graph);

            config.RunActions(ConfigurationType.Explicit, graph);
            config.RunActions(ConfigurationType.Policy, graph);

            // apply the authorization, input, and output nodes
            graph.Behaviors.Each(x => x.InsertNodes(graph.Settings.Get<ConnegSettings>()));

            config.RunActions(ConfigurationType.Reordering, graph);

            // Apply the diagnostic tracing
            new ApplyTracing().Configure(graph);

            htmlConventionCollation.Wait(10.Seconds());
            registerServices(config, graph);

            // TODO -- do something better here.
            Task.WaitAll(layoutAttachmentTasks.Result, 10.Seconds());
            assetDiscovery.Wait(10.Seconds());
            

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
                                () => { x.BuildChains(graph).Each(chain => graph.AddChain(chain)); });
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
                ConfigurationType.Reordering
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
                .Each(x => Activator.CreateInstance(x).As<IFubuRegistryExtension>().Configure(registry));

            types.Where(x => x.CanBeCastTo<IConfigurationAction>()).Each(x => {
                var policy = Activator.CreateInstance(x).As<IConfigurationAction>();
                registry.Policies.Add(policy);
            });
        }

        private static void lookForAccessorOverrides(BehaviorGraph graph)
        {
            graph.Settings.Replace(() => {
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
        }
    }
}