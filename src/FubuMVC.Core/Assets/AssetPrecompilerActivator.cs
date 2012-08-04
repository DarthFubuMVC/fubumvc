using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets
{
    public class AssetPrecompilerActivator : IActivator
    {
        readonly IEnumerable<IAssetPrecompiler> _precompilers;
        readonly AssetGraph _assetGraph;
        readonly IAssetPipeline _assetPipeline;

        public AssetPrecompilerActivator(IEnumerable<IAssetPrecompiler> precompilers, AssetGraph assetGraph, IAssetPipeline assetPipeline)
        {
            _precompilers = precompilers;
            _assetGraph = assetGraph;
            _assetPipeline = assetPipeline;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            _precompilers.Each(x =>
            {
                Action<IAssetRegistration> action = registration => x.Precompile(_assetPipeline, registration);
                _assetGraph.OnPrecompile(action);
            });
        }
    }
}