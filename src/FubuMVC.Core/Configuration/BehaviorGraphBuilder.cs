using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Services;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Security;
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
            graph.ApplicationAssembly = registry.ApplicationAssembly;
            var config = registry.Config;

            config.Local.Settings.RunActions(graph);

            config.Sources.Union(config.Imports)
                .SelectMany(x => x.BuildChains(graph))
                .Each(chain => graph.AddChain(chain));

            // TODO -- clean this up
            config.Local.Explicits.RunActions(graph);
            config.Global.Explicits.RunActions(graph);
            config.Local.Policies.RunActions(graph);
            config.Global.Policies.RunActions(graph);
            config.Local.Reordering.RunActions(graph);
            config.Global.Reordering.RunActions(graph);

            return graph;
        }

        // TOOD -- clean this up a little bit
        public static BehaviorGraph Build(FubuRegistry registry)
        {
            var graph = new BehaviorGraph();
            graph.ApplicationAssembly = registry.ApplicationAssembly;

            var config = registry.Config;

            // TODO -- settings from the application must always win

            // Apply settings
            config.Local.Settings.RunActions(graph);
            config.Global.Settings.RunActions(graph);

            graph.Settings.Alter<ConnegSettings>(x => x.Graph = ConnegGraph.Build(graph));



            var assetDiscovery = AssetSettings.Build(graph);

            var viewDiscovery = graph.Settings.Get<ViewEngineSettings>().BuildViewBag(graph);
            var layoutAttachmentTasks =
                viewDiscovery.ContinueWith(
                    t => graph.Settings.Get<ViewEngineSettings>().Facilities.Select(x => x.LayoutAttachment).ToArray());

            graph.Settings.Replace(viewDiscovery);

            AccessorRulesCompiler.Compile(graph);

            var htmlConventionCollation = HtmlConventionCollator.BuildHtmlConventions(graph);


            config.Add(new ModelBindingServicesRegistry());
            config.Add(new SecurityServicesRegistry());
            config.Add(new HttpStandInServiceRegistry());
            config.Add(new CoreServiceRegistry());
            config.Add(new CachingServiceRegistry());
            config.Add(new UIServiceRegistry());

            discoverChains(config, graph);

            var viewAttachmentTask = viewDiscovery.ContinueWith(t => {
                var attacher = new ViewAttachmentWorker(t.Result, graph.Settings.Get<ViewAttachmentPolicy>());
                attacher.Configure(graph);
            }).ContinueWith(t => {
                new AutoImportModelNamespacesConvention().Configure(graph);
            });

            config.Local.Explicits.RunActions(graph);
            config.Global.Explicits.RunActions(graph);
            config.Local.Policies.RunActions(graph);
            config.Global.Policies.RunActions(graph);

            // apply the authorization, input, and output nodes
            graph.Behaviors.Each(x => x.InsertNodes(graph.Settings.Get<ConnegSettings>()));

            config.Local.Reordering.RunActions(graph);
            config.ApplyGlobalReorderings(graph);

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

    }
}