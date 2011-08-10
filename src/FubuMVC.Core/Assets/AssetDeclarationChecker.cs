using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets
{
    public class AssetDeclarationChecker
    {
        public static readonly string SpecifiedAssetFileDependencyDoesNotExist =
            "Asset file {0} referenced in dependency configuration cannot be found in the application or any package content";

        private readonly IAssetPipeline _pipeline;
        private readonly IPackageLog _log;

        public AssetDeclarationChecker(IAssetPipeline pipeline, IPackageLog log)
        {
            _pipeline = pipeline;
            _log = log;
        }

        // TODO -- would be nice if we could log the provenance of the file dependency.
        // i.e. -- which file had the wrong stuff
        public void VerifyFileDependency(string path)
        {
            var file = _pipeline.Find(path);
            if (file == null)
            {
                _log.MarkFailure(SpecifiedAssetFileDependencyDoesNotExist.ToFormat(path));
            }
        }
    }
}