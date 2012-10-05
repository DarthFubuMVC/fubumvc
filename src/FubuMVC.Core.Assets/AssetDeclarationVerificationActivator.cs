using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.Assets.Diagnostics;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets
{
    public class AssetDeclarationVerificationActivator : IActivator
    {
        private readonly AssetGraph _graph;
        private readonly AssetLogsCache _assetLogs;
        private readonly IAssetFileGraph _fileGraph;

        public static bool Latched { get; set; }

        public AssetDeclarationVerificationActivator(IAssetFileGraph fileGraph, AssetGraph graph, AssetLogsCache assetLogs)
        {
            _fileGraph = fileGraph;
            _graph = graph;
            _assetLogs = assetLogs;
        }

        // No automated tests for this.
        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            var checker = new AssetDeclarationChecker(_fileGraph, log, _assetLogs);
            _graph.AllDependencies().Each(x => checker.VerifyFileDependency(x.Name));
        }

        public override string ToString()
        {
            return "Verify the existence of assets specified in *.asset.config or *.script.config files";
        }
    }
}