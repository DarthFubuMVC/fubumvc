using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets
{
    public interface IAssetRequirements
    {
        IEnumerable<string> AllRequestedAssets { get; }
        IEnumerable<string> AllRenderedAssets { get; }
        void Require(params string[] name);
        void UseAssetIfExists(params string[] names);
        AssetPlanKey DequeueAssetsToRender(MimeType mimeType);
        IEnumerable<AssetPlanKey> DequeueAssetsToRender();
        AssetPlanKey DequeueAssets(MimeType mimeType, params string[] assets);
    }

    public class AssetRequirements : IAssetRequirements
    {
        private readonly IAssetDependencyFinder _finder;
        private readonly IAssetFileGraph _fileGraph;
        private readonly List<string> _rendered = new List<string>();
        private readonly List<string> _requirements = new List<string>();

        public AssetRequirements(IAssetDependencyFinder finder, IAssetFileGraph fileGraph)
        {
            _finder = finder;
            _fileGraph = fileGraph;
        }

        public void Require(params string[] name)
        {
            name.Each(x =>
            {
                // Explode out sets right off the bat
                if (MimeType.MimeTypeByFileName(x) == null )
                {
                    var requiredNames = _finder.CompileDependenciesAndOrder(new string[]{x});
                    _requirements.Fill(requiredNames);
                }
                else
                {
                    _requirements.Fill(x);
                }
            });
        }

        public IEnumerable<string> AllRequestedAssets
        {
            get { return _requirements; }
        }

        public IEnumerable<string> AllRenderedAssets
        {
            get { return _rendered; }
        }

        public void UseAssetIfExists(params string[] names)
        {
            names.Each(name =>
            {
                if (_fileGraph.Find(name) != null)
                {
                    Require(name);
                }
            });
        }

        public AssetPlanKey DequeueAssetsToRender(MimeType mimeType)
        {
            var requested = outstandingAssets()
                .Where(x => MimeType.MimeTypeByFileName(x) == mimeType);

            var names = returnOrderedDependenciesFor(mimeType, requested);
            return new AssetPlanKey(mimeType, names);
        }

        public IEnumerable<AssetPlanKey> DequeueAssetsToRender()
        {
            var mimeTypes = outstandingAssets().Select(MimeType.MimeTypeByFileName).Distinct().ToList();
            return mimeTypes.Select(DequeueAssetsToRender).ToList();
        }

        public AssetPlanKey DequeueAssets(MimeType mimeType, params string[] assets)
        {
            var toBeRendered = assets.Except(_rendered).ToList();
            _rendered.Fill(toBeRendered);
            return new AssetPlanKey(mimeType, toBeRendered);
        }

        private IEnumerable<string> outstandingAssets()
        {
            return _requirements.Except(_rendered).ToList();
        }

        private IEnumerable<string> returnOrderedDependenciesFor(MimeType mimeType, IEnumerable<string> requested)
        {
            var toRender = _finder.CompileDependenciesAndOrder(requested).ToList();
            if (mimeType != null)
            {
                toRender.RemoveAll(x => MimeType.MimeTypeByFileName(x) != mimeType);
            }

            toRender.RemoveAll(x => _rendered.Contains(x));
            _rendered.Fill(toRender);

            return toRender;
        }
    }
}