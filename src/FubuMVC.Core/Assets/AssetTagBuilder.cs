using System;
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
        private readonly IAssetFinder _finder;
        private readonly IHttpRequest _request;

        private readonly Queue<Asset> _queuedScripts = new Queue<Asset>();
        private readonly IList<Asset> _writtenScripts = new List<Asset>(); 

        public AssetTagBuilder(IAssetFinder finder, IHttpRequest request)
        {
            _finder = finder;
            _request = request;
        }

        public IEnumerable<HtmlTag> BuildScriptTags(IEnumerable<string> scripts)
        {
            Func<string, string> toFullUrl = url => _request.ToFullUrl(url);

            while (_queuedScripts.Any())
            {
                var asset = _queuedScripts.Dequeue();
                if (_writtenScripts.Contains(asset)) continue;

                _writtenScripts.Add(asset);

                yield return new ScriptTag(toFullUrl, asset);
            }

            foreach (var x in scripts)
            {
                var asset = _finder.FindAsset(x);

                if (asset == null)
                {
                    yield return new ScriptTag(toFullUrl, null, x);
                }
                else if (!_writtenScripts.Contains(asset))
                {
                    _writtenScripts.Add(asset);
                    yield return new ScriptTag(toFullUrl, asset, x);
                }
            }

        }

        

        public IEnumerable<HtmlTag> BuildStylesheetTags(IEnumerable<string> stylesheets)
        {
            return stylesheets.Select(x =>
            {
                var asset = _finder.FindAsset(x);
                var url = asset == null ? x : asset.Url;

                return new StylesheetLinkTag(_request.ToFullUrl(url));
            });
        }

        public string FindImageUrl(string urlOrFilename)
        {
            var asset = _finder.FindAsset(urlOrFilename);
            var relativeUrl = asset == null ? urlOrFilename : asset.Url;

            return _request.ToFullUrl(relativeUrl);
        }

        public void RequireScript(params string[] scripts)
        {
           scripts
               .Select(x => _finder.FindAsset(x))
               .Where(x => x != null && !_writtenScripts.Contains(x))
               .Each(x => _queuedScripts.Enqueue(x));
        }
    }
}