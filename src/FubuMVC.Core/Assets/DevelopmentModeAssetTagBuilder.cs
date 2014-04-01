using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Http;
using HtmlTags;

namespace FubuMVC.Core.Assets
{
    public class DevelopmentModeAssetTagBuilder : IAssetTagBuilder
    {
        private readonly IAssetGraph _graph;
        private AssetTagBuilder _inner;

        public DevelopmentModeAssetTagBuilder(IAssetGraph graph, IHttpRequest request)
        {
            _graph = graph;

            _inner = new AssetTagBuilder(graph, request);
        }

        public IEnumerable<HtmlTag> BuildScriptTags(IEnumerable<string> scripts)
        {
            var results = find(scripts);

            if (results.Any(x => x.Asset == null))
            {
                throw new MissingAssetException("Requested script(s) {0} cannot be found".ToFormat(Result.MissingListDescription(results)));
            }

            return _inner.BuildScriptTags(scripts);
        }

        private IEnumerable<Result> find(IEnumerable<string> names)
        {
            return names.Select(x => new Result
            {
                Asset = _graph.FindAsset(x),
                Name = x
            }).ToArray();
        }

        public class Result
        {
            public static string MissingListDescription(IEnumerable<Result> results)
            {
                return results.Where(x => x.Asset == null).Select(x => x.Description)
                    .Join(", ");
            }

            public string Name;
            public Asset Asset;

            public string Description
            {
                get
                {
                    return "'" + Name + "'";
                }
            }
        }

        public IEnumerable<HtmlTag> BuildStylesheetTags(IEnumerable<string> stylesheets)
        {
            var results = find(stylesheets);

            if (results.Any(x => x.Asset == null))
            {
                throw new MissingAssetException("Requested stylesheets(s) {0} cannot be found".ToFormat(Result.MissingListDescription(results)));
            }

            return _inner.BuildStylesheetTags(stylesheets);
        }

        public string FindImageUrl(string urlOrFilename)
        {
            var asset = _graph.FindAsset(urlOrFilename);
            if (asset == null)
            {
                throw new MissingAssetException("Requested image '{0}' cannot be found".ToFormat(urlOrFilename));
            }

            return _inner.FindImageUrl(urlOrFilename);
        }
    }
}