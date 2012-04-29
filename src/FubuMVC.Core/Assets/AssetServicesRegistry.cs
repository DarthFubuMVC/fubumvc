using System.Collections.Generic;
using Bottles;
using FubuMVC.Core.Assets.Caching;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Diagnostics;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Assets.Tags;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Resources.Etags;

namespace FubuMVC.Core.Assets
{
    public class AssetServicesRegistry : IFubuRegistryExtension
    {
        // TODO -- make this pluggable
        private readonly AssetContentCache _assetCache = new AssetContentCache();

        public void Configure(FubuRegistry registry)
        {
            registry.Services(addServices);
            registry.Services(addActivators);
            registry.Services(setupAssetCaching);

            registry.Configure(graph =>
            {
                var chain = createAssetContentChain(graph);
                chain.Filters.Add(new AssetEtagInvocationFilter(_assetCache));

                addCaching(chain);
                addWritingAction(chain);
            });
        }

        private void addWritingAction(BehaviorChain chain)
        {
            var call = ActionCall.For<AssetWriter>(x => x.Write(null));
            chain.AddToEnd(call);
        }

        private void addCaching(BehaviorChain chain)
        {
            var cacheNode = new OutputCachingNode{
                ETagCache = ObjectDef.ForValue(_assetCache),
                OutputCache = ObjectDef.ForValue(_assetCache)
            };

            chain.AddToEnd(cacheNode);
        }

        private BehaviorChain createAssetContentChain(BehaviorGraph graph)
        {
            var chain = graph.AddChain();
            var pattern = "_content";
            chain.Route = RouteBuilder.Build(typeof (AssetPath), pattern);
            chain.Route.AddHttpMethodConstraint("GET");
            return chain;
        }

        private void setupAssetCaching(IServiceRegistry registry)
        {
            registry.SetServiceIfNone<IAssetFileChangeListener>(_assetCache);
            registry.SetServiceIfNone<IAssetContentCache>(_assetCache);
            registry.SetServiceIfNone<IAssetFileWatcher, AssetFileWatcher>();
        }

        private static void addServices(IServiceRegistry registry)
        {
            var pipeline = new AssetPipeline();
            registry.SetServiceIfNone<IAssetPipeline>(pipeline);
            registry.SetServiceIfNone<IAssetFileRegistration>(pipeline);

            registry.SetServiceIfNone(new AssetGraph());
            registry.SetServiceIfNone(new AssetLogsCache());

            registry.SetServiceIfNone<IAssetTagWriter, AssetTagWriter>();

            registry.SetServiceIfNone<ICombinationDeterminationService, CombinationDeterminationService>();

            registry.SetServiceIfNone<IAssetCombinationCache, AssetCombinationCache>();
            registry.SetServiceIfNone<IAssetDependencyFinder, AssetDependencyFinderCache>();
            registry.SetServiceIfNone<IAssetTagPlanner, AssetTagPlanner>();
            registry.SetServiceIfNone<IAssetTagBuilder, AssetTagBuilder>();
            registry.SetServiceIfNone<IAssetRequirements, AssetRequirements>();

            registry.SetServiceIfNone<IMissingAssetHandler, TraceOnlyMissingAssetHandler>();

            registry.SetServiceIfNone<IAssetTagPlanCache, AssetTagPlanCache>();

            registry.SetServiceIfNone<ITransformerPolicyLibrary, TransformerPolicyLibrary>();

            registry.SetServiceIfNone<IContentPlanner, ContentPlanner>();
            registry.SetServiceIfNone<IContentPlanCache, ContentPlanCache>();
            registry.SetServiceIfNone<IContentPipeline, ContentPipeline>();

            registry.SetServiceIfNone<IContentWriter, ContentWriter>();

            registry.SetServiceIfNone<IETagGenerator<IEnumerable<AssetFile>>, AssetFileEtagGenerator>();
        }

        private static void addActivators(IServiceRegistry registry)
        {
            registry.FillType(typeof (IActivator), typeof (AssetGraphConfigurationActivator));
            registry.FillType(typeof (IActivator), typeof (AssetPipelineBuilderActivator));
            registry.FillType(typeof (IActivator), typeof (AssetDeclarationVerificationActivator));
            registry.FillType(typeof (IActivator), typeof (MimetypeRegistrationActivator));
            registry.FillType(typeof (IActivator), typeof (AssetCombinationBuildingActivator));
            registry.FillType(typeof (IActivator), typeof (AssetPolicyActivator));
            registry.FillType(typeof (IActivator), typeof (AssetFileWatchingActivator));
        }
    }
}