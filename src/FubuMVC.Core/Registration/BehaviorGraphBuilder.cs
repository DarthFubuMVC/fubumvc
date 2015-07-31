using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime.Files;
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

            var graph = new BehaviorGraph
            {
                ApplicationAssembly = registry.ApplicationAssembly,
                PackageAssemblies = packageAssemblies
            };

            var accessorRules = AccessorRulesCompiler.Compile(graph, perfTimer);


            var config = registry.Config;

            perfTimer.Record("Applying Settings", () => applySettings(config, graph, diagnostics, files));

            var featureLoading = featureLoader.ApplyAll(graph.Settings, registry);

            featureLoading.Wait();
            Task.WaitAll(featureLoading.Result);

            perfTimer.Record("Local Application BehaviorGraph", () => config.BuildLocal(graph, perfTimer));

            perfTimer.Record("Explicit Configuration", () => config.Global.Explicits.RunActions(graph));
            perfTimer.Record("Global Policies", () => config.Global.Policies.RunActions(graph));

            perfTimer.Record("Inserting Conneg and Authorization Nodes",
                () => insertConnegAndAuthorizationNodes(graph));

            perfTimer.Record("Applying Global Reorderings", () => config.ApplyGlobalReorderings(graph));


            perfTimer.Record("Applying Tracing", () => applyTracing(graph));

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

        private static void applySettings(ConfigGraph config, BehaviorGraph graph, IActivationDiagnostics diagnostics, IFubuApplicationFiles files)
        {
            // Might come back to this.
            config.Imports.Each(x => x.InitializeSettings(graph));
            config.Settings.Each(x => x.Alter(graph.Settings));

            var viewSettings = graph.Settings.Get<ViewEngineSettings>();
            

            var views = viewSettings.BuildViewBag(graph, diagnostics, files)
                .ContinueWith(t =>
                {
                    return viewSettings.Profiles(t.Result);
                });

            var conneg = graph.Settings.Get<ConnegSettings>();
            

            conneg.ReadConnegGraph(graph);
            conneg.StoreViews(views);
        }
    }
}