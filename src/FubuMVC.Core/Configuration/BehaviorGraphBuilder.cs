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

            // TODO -- settings from the application must always win

            // Apply settings

            config.Imports.Each(x => x.InitializeSettings(graph));
            config.Settings.Each(x => x.Alter(graph.Settings));

            graph.Settings.Alter<ConnegSettings>(x => x.Graph = ConnegGraph.Build(graph));


            var assetDiscovery = AssetSettings.Build(graph);

            var viewDiscovery = graph.Settings.Get<ViewEngineSettings>().BuildViewBag(graph);
            var layoutAttachmentTasks =
                viewDiscovery.ContinueWith(
                    t => graph.Settings.Get<ViewEngineSettings>().Facilities.Select(x => x.LayoutAttachment).ToArray());

            graph.Settings.Replace(viewDiscovery);

            AccessorRulesCompiler.Compile(graph);

            var htmlConventionCollation = HtmlConventionCollator.BuildHtmlConventions(graph);


            config.DiscoverChains(graph);


            viewDiscovery.ContinueWith(t => {
                var attacher = new ViewAttachmentWorker(t.Result, graph.Settings.Get<ViewAttachmentPolicy>());
                attacher.Configure(graph);
            }).ContinueWith(t => { new AutoImportModelNamespacesConvention().Configure(graph); }).Wait(10.Seconds());

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

            // TODO -- this is terrible. Do something to do the waits better
            htmlConventionCollation.Wait(10.Seconds());
            //viewAttachmentTask.Wait(10.Seconds());

            config.RegisterServices(graph);

            // TODO -- do something better here.
            Task.WaitAll(layoutAttachmentTasks.Result, 10.Seconds());
            assetDiscovery.Wait(10.Seconds());


            return graph;
        }

    }
}