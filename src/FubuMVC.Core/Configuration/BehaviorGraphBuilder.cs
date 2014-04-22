using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            if (FubuMode.InDevelopment())
            {
                graph.AddChain(RoutedChain.For<AboutDiagnostics>(x => x.get__about(), "_about"));
                graph.AddChain(RoutedChain.For<AboutDiagnostics>(x => x.get__loaded(), "_loaded"));
            }

            // Apply settings
            applySettings(config, graph);


            var assetDiscovery = AssetSettings.Build(graph);

            var viewDiscovery = graph.Settings.Get<ViewEngineSettings>().BuildViewBag(graph);
            var layoutAttachmentTasks =
                viewDiscovery.ContinueWith(
                    t => graph.Settings.Get<ViewEngineSettings>().Facilities.Select(x => x.LayoutAttachment).ToArray());

            graph.Settings.Replace(viewDiscovery);

            AccessorRulesCompiler.Compile(graph);

            var htmlConventionCollation = HtmlConventionCollator.BuildHtmlConventions(graph);


            config.BuildLocal(graph);


            viewDiscovery.ContinueWith(t => {
                var attacher = new ViewAttachmentWorker(t.Result, graph.Settings.Get<ViewAttachmentPolicy>());
                attacher.Configure(graph);
            }).ContinueWith(t => { new AutoImportModelNamespacesConvention().Configure(graph); }).Wait(10.Seconds());

            config.Global.Explicits.RunActions(graph);
            config.Global.Policies.RunActions(graph);

            // apply the authorization, input, and output nodes
            graph.Behaviors.Each(x => x.InsertNodes(graph.Settings.Get<ConnegSettings>()));

            config.ApplyGlobalReorderings(graph);

            // Apply the diagnostic tracing
            new ApplyTracing().Configure(graph);

            // Wait until all the other threads are done.
            var registration = htmlConventionCollation.ContinueWith(t => config.RegisterServices(graph));
            Task.WaitAll(registration, layoutAttachmentTasks, assetDiscovery);
            Task.WaitAll(layoutAttachmentTasks.Result);


            return graph;
        }

        private static void applySettings(ConfigGraph config, BehaviorGraph graph)
        {
            config.Imports.Each(x => x.InitializeSettings(graph));
            config.Settings.Each(x => x.Alter(graph.Settings));
            graph.Settings.Alter<ConnegSettings>(x => x.Graph = ConnegGraph.Build(graph));
        }
    }
}