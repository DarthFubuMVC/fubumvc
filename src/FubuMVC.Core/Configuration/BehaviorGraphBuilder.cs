using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bottles;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core.Configuration
{
    internal static class BehaviorGraphBuilder
    {


        // TOOD -- clean this up a little bit
        public static BehaviorGraph Build(FubuRegistry registry)
        {
            var graph = new BehaviorGraph {ApplicationAssembly = registry.ApplicationAssembly};
            var config = registry.Config;

            PackageRegistry.Timer.Record("Applying Settings", () => applySettings(config, graph));

            var viewDiscovery = graph.Settings.Get<ViewEngineSettings>().BuildViewBag(graph);
            var layoutAttachmentTasks =
                viewDiscovery.ContinueWith(
                    t => graph.Settings.Get<ViewEngineSettings>().Facilities.Select(x => x.LayoutAttachment).ToArray());

            graph.Settings.Replace(viewDiscovery);

            AccessorRulesCompiler.Compile(graph);

            var htmlConventionCollation = HtmlConventionCollator.BuildHtmlConventions(graph);

            addBuiltInDiagnostics(graph);

            PackageRegistry.Timer.Record("Local Application BehaviorGraph", () => config.BuildLocal(graph));

            viewDiscovery.RecordContinuation("View Attachment",t =>
            {
                var attacher = new ViewAttachmentWorker(t.Result, graph.Settings.Get<ViewAttachmentPolicy>());
                attacher.Configure(graph);
            }).Wait();


            PackageRegistry.Timer.Record("Explicit Configuration", () => config.Global.Explicits.RunActions(graph));
            PackageRegistry.Timer.Record("Global Policies", () => config.Global.Policies.RunActions(graph));

            PackageRegistry.Timer.Record("Inserting Conneg and Authorization Nodes",
                () => insertConnegAndAuthorizationNodes(graph));

            PackageRegistry.Timer.Record("Applying Global Reorderings", () => config.ApplyGlobalReorderings(graph));


            PackageRegistry.Timer.Record("Applying Tracing", () => applyTracing(graph));

            // Wait until all the other threads are done.
            var registration = htmlConventionCollation.ContinueWith(t => config.RegisterServices(graph));
            Task.WaitAll(registration, layoutAttachmentTasks);
            Task.WaitAll(layoutAttachmentTasks.Result);

            new AutoImportModelNamespacesConvention().Configure(graph);

            return graph;
        }

        private static void addBuiltInDiagnostics(BehaviorGraph graph)
        {
            if (FubuMode.InDevelopment())
            {
                graph.AddChain(RoutedChain.For<AboutDiagnostics>(x => x.get__about(), "_about"));
                graph.AddChain(RoutedChain.For<AboutDiagnostics>(x => x.get__loaded(), "_loaded"));
            }
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
            graph.Settings.Alter<ConnegSettings>(x => x.Graph = ConnegGraph.Build(graph));
        }
    }
}