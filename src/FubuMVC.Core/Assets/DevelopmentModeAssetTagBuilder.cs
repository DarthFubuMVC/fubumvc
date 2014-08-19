using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Http;
using HtmlTags;

namespace FubuMVC.Core.Assets
{
    public class DevelopmentModeAssetTagBuilder : IAssetTagBuilder
    {
        private readonly IAssetFinder _finder;
        private readonly AssetTagBuilder _inner;

        public DevelopmentModeAssetTagBuilder(IAssetFinder finder, IHttpRequest request)
        {
            _finder = finder;

            _inner = new AssetTagBuilder(finder, request);
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
                Asset = _finder.FindAsset(x),
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
            var asset = _finder.FindAsset(urlOrFilename);
            if (asset == null)
            {
                throw new MissingAssetException("Requested image '{0}' cannot be found".ToFormat(urlOrFilename));
            }

            return _inner.FindImageUrl(urlOrFilename);
        }

        public void RequireScript(params string[] scripts)
        {
            _inner.RequireScript(scripts);
        }
    }
}