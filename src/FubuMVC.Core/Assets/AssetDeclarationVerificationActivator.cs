using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets
{
    public class AssetDeclarationVerificationActivator : IActivator
    {
        private readonly AssetGraph _graph;
        private readonly AssetPipeline _pipeline;

        public AssetDeclarationVerificationActivator(AssetPipeline pipeline, AssetGraph graph)
        {
            _pipeline = pipeline;
            _graph = graph;
        }

        // No automated tests for this.
        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            var checker = new AssetDeclarationChecker(_pipeline, log);
            _graph.AllDependencies().Each(x => checker.VerifyFileDependency(x.Name));
        }

        public override string ToString()
        {
            return "Verify the existence of assets specified in *.asset.config or *.script.config files";
        }
    }
}