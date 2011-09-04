using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets
{
    public interface IAssetPolicy
    {
        void Apply(IPackageLog log, IAssetPipeline pipeline, AssetGraph graph);
    }

    public class AssetPolicyActivator : IActivator
    {
        private readonly IEnumerable<IAssetPolicy> _policies;
        private readonly IAssetPipeline _pipeline;
        private readonly AssetGraph _graph;

        public AssetPolicyActivator(IEnumerable<IAssetPolicy> policies, IAssetPipeline pipeline, AssetGraph graph)
        {
            _policies = policies;
            _pipeline = pipeline;
            _graph = graph;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            _policies.Each(p =>
            {
                log.Trace("Running " + p);
                p.Apply(log, _pipeline, _graph);
            });
        }
    }
}