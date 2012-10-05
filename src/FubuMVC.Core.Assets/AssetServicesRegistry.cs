using System;
using System.Collections.Generic;
using Bottles;
using FubuMVC.Core.Assets.Caching;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Diagnostics;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Assets.Tags;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Etags;

namespace FubuMVC.Core.Assets
{
    public class AssetServicesRegistry : ServiceRegistry
    {
        public AssetServicesRegistry()
        {
            var pipeline = new AssetFileGraph();
            SetServiceIfNone<IAssetFileGraph>(pipeline);
            SetServiceIfNone<IAssetFileRegistration>(pipeline);

            SetServiceIfNone(new AssetGraph());
            SetServiceIfNone(new AssetLogsCache());

            SetServiceIfNone<IAssetCacheHeaders, AssetCacheHeaders>();

            SetServiceIfNone<IAssetTagWriter, AssetTagWriter>();

            SetServiceIfNone<ICombinationDeterminationService, CombinationDeterminationService>();

            SetServiceIfNone<IAssetCombinationCache, AssetCombinationCache>();
            SetServiceIfNone<IAssetDependencyFinder, AssetDependencyFinderCache>();
            SetServiceIfNone<IAssetTagPlanner, AssetTagPlanner>();
            SetServiceIfNone<IAssetTagBuilder, AssetTagBuilder>();
            SetServiceIfNone<IAssetRequirements, AssetRequirements>();

            SetServiceIfNone<IMissingAssetHandler, TraceOnlyMissingAssetHandler>();

            SetServiceIfNone<IAssetTagPlanCache, AssetTagPlanCache>();

            SetServiceIfNone<ITransformerPolicyLibrary, TransformerPolicyLibrary>();

            SetServiceIfNone<IContentPlanner, ContentPlanner>();
            SetServiceIfNone<IContentPlanCache, ContentPlanCache>();
            SetServiceIfNone<IContentPipeline, ContentPipeline>();

            SetServiceIfNone<IContentWriter, ContentWriter>();

            SetServiceIfNone<IETagGenerator<IEnumerable<AssetFile>>, AssetFileEtagGenerator>();

            SetServiceIfNone<IAssetContentCache, AssetContentCache>();
            SetServiceIfNone<IAssetFileChangeListener, AssetFileChangeListener>();
            SetServiceIfNone<IAssetFileWatcher, AssetFileWatcher>();

            FillType(typeof (IActivator), typeof (AssetPrecompilerActivator));
            FillType(typeof(IActivator), typeof(AssetGraphConfigurationActivator));
            FillType(typeof(IActivator), typeof(AssetFileGraphBuilderActivator));
            FillType(typeof(IActivator), typeof(AssetDeclarationVerificationActivator));
            FillType(typeof(IActivator), typeof(MimetypeRegistrationActivator));
            FillType(typeof(IActivator), typeof(AssetCombinationBuildingActivator));
            FillType(typeof(IActivator), typeof(AssetPolicyActivator));
            FillType(typeof(IActivator), typeof(AssetFileWatchingActivator));

            SetServiceIfNone<IAssetUrls, AssetUrls>();
        }
    }
}