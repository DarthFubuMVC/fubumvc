using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using FubuMVC.Core.Http;
using HtmlTags;

namespace FubuMVC.Core.Assets
{
    // Tested through integration tests
    public class AssetTagBuilder : IAssetTagBuilder
    {
        private readonly IAssetGraph _graph;
        private readonly IHttpRequest _request;

        public AssetTagBuilder(IAssetGraph graph, IHttpRequest request)
        {
            _graph = graph;
            _request = request;
        }

        public IEnumerable<HtmlTag> BuildScriptTags(IEnumerable<string> scripts)
        {
            return scripts.Select(x => {
                var asset = _graph.FindAsset(x);

                return new ScriptTag(url => _request.ToFullUrl(url), asset, x);
            });
        }

        

        public IEnumerable<HtmlTag> BuildStylesheetTags(IEnumerable<string> stylesheets)
        {
            return stylesheets.Select(x =>
            {
                var asset = _graph.FindAsset(x);
                var url = asset == null ? x : asset.Url;

                return new StylesheetLinkTag(_request.ToFullUrl(url));
            });
        }

        public string FindImageUrl(string urlOrFilename)
        {
            var asset = _graph.FindAsset(urlOrFilename);
            var relativeUrl = asset == null ? urlOrFilename : asset.Url;

            return _request.ToFullUrl(relativeUrl);
        }
    }
}