using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Diagnostics
{
    public class AssetFubuDiagnostics
    {
        private readonly IAssetFinder _assets;

        public AssetFubuDiagnostics(IAssetFinder assets)
        {
            _assets = assets;
        }

        public AssetToken get_assets_find_Name(AssetSearch search)
        {
            var asset = _assets.FindAsset(search.Name);
            return asset == null ? new AssetToken {url = search.Name, file = "NOT FOUND"} : new AssetToken(asset);
        }

        public Dictionary<string, object> get_assets()
        {
            return new Dictionary<string, object>
            {
                {"assets", _assets.FindAll().Assets.Select(x => new AssetToken(x)).ToArray()}
            };
        }
    }

    public class AssetSearch
    {
        public string Name { get; set; }
    }

    public class AssetToken
    {
        public string url;
        public string mimetype;
        public string file;
        public string cdn;
        public string provenance;

        public AssetToken()
        {
        }

        public AssetToken(Asset asset)
        {
            url = asset.Url;
            mimetype = asset.MimeType.Value;
            file = asset.Filename;
            cdn = asset.CdnUrl;
            provenance = asset.File.Provenance;
        }
    }
}