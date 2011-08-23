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
        void UseFileIfExists(string name);
        IEnumerable<string> DequeueAssetsToRender(MimeType mimeType);
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

        public void UseFileIfExists(string name)
        {
            if (_pipeline.Find(name) != null)
            {
                Require(name);
            }
        }

        private IEnumerable<string> outstandingAssets()
        {
            return _requirements.Except(_rendered).ToList();
        }

        public IEnumerable<string> DequeueAssetsToRender(MimeType mimeType)
        {
            var requested = outstandingAssets()
                .Where(x => MimeType.DetermineMimeTypeFromName(x) == mimeType);

            return returnOrderedDependenciesFor(requested);
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
            return outstandingAssets()
                .GroupBy(MimeType.DetermineMimeTypeFromName)
                .Select(@group =>
                {
                    _rendered.AddRange(group);
                    return new AssetPlanKey(@group.Key, @group);
                }).ToList();
        }
    }
}