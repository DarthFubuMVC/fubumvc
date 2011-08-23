using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Content;
using System.Linq;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets
{
    public interface IAssetRequirements
    {
        void Require(params string[] name);
        IEnumerable<string> AllRequestedAssets { get; }
        IEnumerable<string> AllRenderedAssets { get; }
        void UseAssetIfExists(params string[] names);
        AssetPlanKey DequeueAssetsToRender(MimeType mimeType);  
        IEnumerable<AssetPlanKey> DequeueAssetsToRender();
    }

    public class AssetRequirements : IAssetRequirements
    {
        private readonly IAssetDependencyFinder _finder;
        private readonly IAssetPipeline _pipeline;
        private readonly List<string> _requirements = new List<string>();
        private readonly List<string> _rendered = new List<string>();

        public AssetRequirements(IAssetDependencyFinder finder, IAssetPipeline pipeline)
        {
            _finder = finder;
            _pipeline = pipeline;
        }

        public void Require(params string[] name)
        {
            _requirements.Fill(name);
        }

        public IEnumerable<string> AllRequestedAssets
        {
            get
            {
                return _requirements;
            }
        }

        public IEnumerable<string> AllRenderedAssets
        {
            get
            {
                return _rendered;
            }
        }

        public void UseAssetIfExists(params string[] names)
        {
            names.Each(name =>
            {
                if (_pipeline.Find(name) != null)
                {
                    Require(name);
                }
            });
        }

        private IEnumerable<string> outstandingAssets()
        {
            return _requirements.Except(_rendered).ToList();
        }

        public AssetPlanKey DequeueAssetsToRender(MimeType mimeType)
        {
            var requested = outstandingAssets()
                .Where(x => MimeType.DetermineMimeTypeFromName(x) == mimeType);

            var names = returnOrderedDependenciesFor(requested);
            return new AssetPlanKey(mimeType, names);
        }

        private IEnumerable<string> returnOrderedDependenciesFor(IEnumerable<string> requested)
        {
            var toRender = _finder.CompileDependenciesAndOrder(requested).ToList();
            toRender.RemoveAll(x => _rendered.Contains(x));
            _rendered.Fill(toRender);

            return toRender;
        }

        public IEnumerable<AssetPlanKey> DequeueAssetsToRender()
        {
            var mimeTypes = outstandingAssets().Select(MimeType.DetermineMimeTypeFromName).Distinct().ToList();
            return mimeTypes.Select(DequeueAssetsToRender).ToList();
        }
    }
}