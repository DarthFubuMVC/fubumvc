using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Assets
{
    public class AssetGraph
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

        public Asset RegisterCdnAsset(CdnAsset cdn)
        {
            var file = cdn.Filename();
            var asset = _searches[file];
            if (asset == null)
            {
                asset = new Asset(cdn.Url)
                {

                };
                _assets.Add(asset);
            }

            asset.CdnUrl = cdn.Url;
            asset.FallbackTest = cdn.Fallback;

            return asset;

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

        public void StoreAlias(string alias, string filename)
        {
            var asset = findAsset(filename);
            if (asset == null)
            {
                throw new ArgumentOutOfRangeException("filename","No asset file named '{0}' can be found".ToFormat(filename));
            }

            _searches[alias] = asset;
        }
    }
}