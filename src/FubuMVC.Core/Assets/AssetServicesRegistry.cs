using System;
using System.Collections.Generic;
using Bottles;
using FubuMVC.Core.Assets.Caching;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Assets.Tags;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Etags;

namespace FubuMVC.Core.Assets
{
    public class AssetServicesRegistry : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services(addServices);
            registry.Services(addActivators);
        }

        private static void addServices(IServiceRegistry registry)
        {
            var pipeline = new AssetPipeline();
            registry.SetServiceIfNone<IAssetPipeline>(pipeline);
            registry.SetServiceIfNone<IAssetFileRegistration>(pipeline);

            registry.SetServiceIfNone(new AssetGraph());

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

            // TODO -- make this pluggable
            var cache = new AssetContentCache();

            registry.SetServiceIfNone<IAssetFileChangeListener>(cache);
            registry.SetServiceIfNone<IAssetFileWatcher, AssetFileWatcher>();
        }

        private static void addActivators(IServiceRegistry registry)
        {
            registry.FillType(typeof(IActivator), typeof(AssetGraphConfigurationActivator));
            registry.FillType(typeof(IActivator), typeof(AssetPipelineBuilderActivator));
            registry.FillType(typeof(IActivator), typeof(AssetDeclarationVerificationActivator));
            registry.FillType(typeof(IActivator), typeof(MimetypeRegistrationActivator));
            registry.FillType(typeof(IActivator), typeof(AssetCombinationBuildingActivator));
            registry.FillType(typeof(IActivator), typeof(AssetPolicyActivator));
            registry.FillType(typeof(IActivator), typeof(AssetFileWatchingActivator));
        }
    }
}