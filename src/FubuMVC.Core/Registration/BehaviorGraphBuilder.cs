using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuMVC.Core.ServiceBus.Subscriptions;
using FubuMVC.Core.View;

namespace FubuMVC.Core.Registration
{
    internal static class BehaviorGraphBuilder
    {
        public static BehaviorGraph Build(FubuRegistry registry, IPerfTimer perfTimer,
            IEnumerable<Assembly> packageAssemblies, IActivationDiagnostics diagnostics, IFubuApplicationFiles files)
        {
            var featureLoader = new FeatureLoader();
            featureLoader.LookForFeatures();

            if (registry.Mode.InDevelopment())
            {
                registry.AlterSettings<DiagnosticsSettings>(_ => _.TraceLevel = TraceLevel.Verbose);
                registry.AlterSettings<AssetSettings>(_ => _.SetupForDevelopment());
            }

            
            

            var graph = new BehaviorGraph
            {
                ApplicationAssembly = registry.ApplicationAssembly,
                PackageAssemblies = packageAssemblies
            };

            var accessorRules = AccessorRulesCompiler.Compile(graph, perfTimer);



            var config = registry.Config;

            perfTimer.Record("Applying Settings", () => applySettings(config, graph, diagnostics, files));

            perfTimer.Record("Enable built in polling jobs", () => enableBuiltInJobs(graph));
            perfTimer.Record("Applying Feature Settings", () => featureLoader.ApplyAll(graph.Settings, registry).Wait());

            perfTimer.Record("Local Application BehaviorGraph", () => config.BuildLocal(graph, perfTimer));

            perfTimer.Record("Explicit Configuration", () => config.Global.Explicits.RunActions(graph));
            perfTimer.Record("Global Policies", () => config.Global.Policies.RunActions(graph));

            perfTimer.Record("Inserting Conneg and Authorization Nodes",
                () => insertConnegAndAuthorizationNodes(graph));

            perfTimer.Record("Applying Global Reorderings", () => config.ApplyGlobalReorderings(graph));


            if (registry.Mode.InDevelopment() || graph.Settings.Get<DiagnosticsSettings>().TraceLevel != TraceLevel.None)
            {
                perfTimer.Record("Applying Tracing", () => ApplyTracing.Configure(graph));
            }

            accessorRules.Wait();

            new AutoImportModelNamespacesConvention().Configure(graph);

            return graph;
        }

        private static void enableBuiltInJobs(BehaviorGraph graph)
        {
            if (graph.Settings.Get<TransportSettings>().Enabled)
            {
                var jobs = graph.Settings.Get<PollingJobSettings>();

                jobs.AddJob(PollingJobChain.For<DelayedEnvelopeProcessor, TransportSettings>(x => x.DelayMessagePolling));
                jobs.AddJob(PollingJobChain.For<ExpiringListenerCleanup, TransportSettings>(x => x.ListenerCleanupPolling));
                jobs.AddJob(PollingJobChain.For<HealthMonitorPollingJob, HealthMonitoringSettings>(x => x.Interval));
                jobs.AddJob(PollingJobChain.For<SubscriptionRefreshJob, TransportSettings>(x => x.SubscriptionRefreshPolling));


            }
        }


        private static void insertConnegAndAuthorizationNodes(BehaviorGraph graph)
        {
            graph.Chains.Each(x => x.InsertNodes(graph.Settings.Get<ConnegSettings>()));
        }

        private static void applySettings(ConfigGraph config, BehaviorGraph graph, IActivationDiagnostics diagnostics, IFubuApplicationFiles files)
        {
            // Might come back to this.
            config.Imports.Each(x => x.InitializeSettings(graph));
            config.Settings.Each(x => x.Alter(graph.Settings));

            var viewSettings = graph.Settings.Get<ViewEngineSettings>();


            var views = viewSettings.BuildViewBag(graph, diagnostics, files);

            var conneg = graph.Settings.Get<ConnegSettings>();
            

            conneg.ReadConnegGraph(graph);
            conneg.StoreViews(views);
        }
    }
}