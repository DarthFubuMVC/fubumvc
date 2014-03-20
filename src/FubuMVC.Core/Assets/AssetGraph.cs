using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuMVC.Core.Assets
{
    public class AssetGraph : IAssetGraph
    {
        private readonly IList<Asset> _assets = new List<Asset>();
        private readonly ConcurrentCache<string, Asset> _searches = new ConcurrentCache<string, Asset>();

        public AssetGraph()
        {
            _searches.OnMissing = findAsset;
        }

        public void Add(Asset asset)
        {
            _assets.Add(asset);
        }

        public void Add(IEnumerable<Asset> assets)
        {
            _assets.AddRange(assets);
        }

        public Asset FindAsset(string search)
        {
            return _searches[search];
        }

        private Asset findAsset(string search)
        {
            search = search.TrimStart('/');

            var exact = _assets.FirstOrDefault(x => x.Url == search);
            if (exact != null) return exact;

            if (search.Contains("/"))
            {
                var pathSearch = "/" + search;
                return _assets.FirstOrDefault(x => x.Url.EndsWith(pathSearch));
            }

            return _assets.FirstOrDefault(x => x.Filename == search);
        }

        public IEnumerable<Asset> Assets
        {
            get { return _assets; }
        }
    }
}