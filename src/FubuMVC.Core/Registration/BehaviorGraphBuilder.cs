using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core.Registration
{
    internal static class BehaviorGraphBuilder
    {
        // TOOD -- clean this up a little bit
        public static BehaviorGraph Build(FubuRegistry registry, IPerfTimer perfTimer, IEnumerable<Assembly> packageAssemblies, IActivationDiagnostics diagnostics, IFubuApplicationFiles files)
        {
            var featureLoader = new FeatureLoader();
            featureLoader.LookForFeatures();

            var graph = new BehaviorGraph
            {
                ApplicationAssembly = registry.ApplicationAssembly,
                PackageAssemblies = packageAssemblies,
                Diagnostics = diagnostics
            };
            var config = registry.Config;

            perfTimer.Record("Applying Settings", () => applySettings(config, graph));

            var viewDiscovery = graph.Settings.Get<ViewEngineSettings>().BuildViewBag(graph, perfTimer, files);
            var layoutAttachmentTasks =
                viewDiscovery.ContinueWith(
                    t => graph.Settings.Get<ViewEngineSettings>().Facilities.Select(x => x.LayoutAttachment).ToArray());

            graph.Settings.Replace(viewDiscovery);

            var accessorRules = AccessorRulesCompiler.Compile(graph, perfTimer);

            var featureLoading = featureLoader.ApplyAll(graph.Settings, registry);
            featureLoading.Wait();
            Task.WaitAll(featureLoading.Result);

            perfTimer.Record("Local Application BehaviorGraph", () => config.BuildLocal(graph, perfTimer));

            // TODO -- undo the R# change to something. Or other.
            viewDiscovery.ContinueWith(t1 => perfTimer.Record("View Attachment", () => ((Action<Task<ViewBag>>) (t =>
            {
                var attacher = new ViewAttachmentWorker(t.Result, graph.Settings.Get<ViewAttachmentPolicy>());
                attacher.Configure(graph);
            }))(t1))).Wait();


            


            perfTimer.Record("Explicit Configuration", () => config.Global.Explicits.RunActions(graph));
            perfTimer.Record("Global Policies", () => config.Global.Policies.RunActions(graph));

            perfTimer.Record("Inserting Conneg and Authorization Nodes",
                () => insertConnegAndAuthorizationNodes(graph));

            perfTimer.Record("Applying Global Reorderings", () => config.ApplyGlobalReorderings(graph));


            perfTimer.Record("Applying Tracing", () => applyTracing(graph));

            // TODO -- just make this all async
            layoutAttachmentTasks.Wait();
            Task.WaitAll(layoutAttachmentTasks.Result);

            accessorRules.Wait();

            new AutoImportModelNamespacesConvention().Configure(graph);

            return graph;
        }


        private static void applyTracing(BehaviorGraph graph)
        {
            new ApplyTracing().Configure(graph);
        }

        private static void insertConnegAndAuthorizationNodes(BehaviorGraph graph)
        {
            graph.Behaviors.Each(x => x.InsertNodes(graph.Settings.Get<ConnegSettings>()));
        }

        private static void applySettings(ConfigGraph config, BehaviorGraph graph)
        {
            config.Imports.Each(x => x.InitializeSettings(graph));
            config.Settings.Each(x => x.Alter(graph.Settings));
            graph.Settings.Alter<ConnegSettings>(x => x.ReadConnegGraph(graph));
        }
    }
}