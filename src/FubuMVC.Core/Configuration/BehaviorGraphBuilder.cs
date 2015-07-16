using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore.Binding.InMemory;
using FubuCore.Logging;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core.Configuration
{
    internal static class BehaviorGraphBuilder
    {
        // TOOD -- clean this up a little bit
        public static BehaviorGraph Build(FubuRegistry registry, IPerfTimer perfTimer, IEnumerable<Assembly> packageAssemblies, IBottlingDiagnostics diagnostics)
        {
            var graph = new BehaviorGraph
            {
                ApplicationAssembly = registry.ApplicationAssembly,
                PackageAssemblies = packageAssemblies,
                Diagnostics = diagnostics
            };
            var config = registry.Config;

            perfTimer.Record("Applying Settings", () => applySettings(config, graph));

            var viewDiscovery = graph.Settings.Get<ViewEngineSettings>().BuildViewBag(graph, perfTimer);
            var layoutAttachmentTasks =
                viewDiscovery.ContinueWith(
                    t => graph.Settings.Get<ViewEngineSettings>().Facilities.Select(x => x.LayoutAttachment).ToArray());

            graph.Settings.Replace(viewDiscovery);

            AccessorRulesCompiler.Compile(graph, perfTimer);


            addBuiltInDiagnostics(graph, perfTimer);

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

            // Wait until all the other threads are done.
            var registration = Task.Factory.StartNew(() => config.RegisterServices(graph));

            Task.WaitAll(registration, layoutAttachmentTasks);
            Task.WaitAll(layoutAttachmentTasks.Result);

            new AutoImportModelNamespacesConvention().Configure(graph);

            return graph;
        }

        private static void addBuiltInDiagnostics(BehaviorGraph graph, IPerfTimer timer)
        {
            var settings = graph.Settings.Get<DiagnosticsSettings>();
            if (FubuMode.InDevelopment() || settings.TraceLevel != TraceLevel.None)
            {
                var chains = new DiagnosticChainsSource().BuildChains(graph, timer).ToArray();

                // Apply authorization rules to the diagnostic chains
                chains.Each(x => x.Authorization.AddPolicies(settings.AuthorizationRights));

                graph.AddChains(chains);
            }


            if (FubuMode.InDevelopment() || settings.TraceLevel == TraceLevel.Verbose)
            {
                graph.Services.Clear(typeof (IBindingLogger));
                graph.Services.AddService<IBindingLogger, RecordingBindingLogger>();

                graph.Services.Clear(typeof (IBindingHistory));
                graph.Services.AddService<IBindingHistory, BindingHistory>();

                graph.Services.AddService<ILogListener, RequestTraceListener>();
            }
            else if (settings.TraceLevel == TraceLevel.Production)
            {
                graph.Services.AddService<ILogListener, ProductionModeTraceListener>();
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