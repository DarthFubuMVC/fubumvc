using System.Collections.Generic;
using System.Linq;
using HtmlTags;

namespace FubuMVC.Core.Assets
{
    // Tested through integration tests
    public class AssetTagBuilder : IAssetTagBuilder
    {
        private readonly IAssetGraph _graph;

        public AssetTagBuilder(IAssetGraph graph)
        {
            _graph = graph;
        }

        public IEnumerable<HtmlTag> BuildScriptTags(IEnumerable<string> scripts)
        {
            return scripts.Select(x => {
                var asset = _graph.FindAsset(x);
                var url = asset == null ? x : asset.Url;

                return new ScriptTag(url);
            });
        }

        

        public IEnumerable<HtmlTag> BuildStylesheetTags(IEnumerable<string> stylesheets)
        {
            return stylesheets.Select(x =>
            {
                var asset = _graph.FindAsset(x);
                var url = asset == null ? x : asset.Url;

                return new StylesheetLinkTag(url);
            });
        }

        public string FindImageUrl(string urlOrFilename)
        {
            var asset = _graph.FindAsset(urlOrFilename);
            return asset == null ? urlOrFilename : asset.Url;
        }
    }
}