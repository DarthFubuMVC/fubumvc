using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuMVC.Core.Assets
{
    public class AssetDependencyFinderCache : IAssetDependencyFinder
    {
        private readonly AssetGraph _graph;
        private readonly Cache<AssetNamesKey, IEnumerable<string>> _dependencies;

        public AssetDependencyFinderCache(AssetGraph graph)
        {
            _graph = graph;
            _dependencies = new Cache<AssetNamesKey, IEnumerable<string>>(FindDependencies);
        }

        public IEnumerable<string> CompileDependenciesAndOrder(IEnumerable<string> names)
        {
            return _dependencies[new AssetNamesKey(names)];
        }

        public virtual IEnumerable<string> FindDependencies(AssetNamesKey key)
        {
            return _graph.GetAssets(key.Names).Select(x => x.Name);
        }
    }
}