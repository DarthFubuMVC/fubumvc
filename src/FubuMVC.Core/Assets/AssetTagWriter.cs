using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Content;
using HtmlTags;

namespace FubuMVC.Core.Assets
{
    public class AssetTagWriter : IAssetTagWriter
    {
        private readonly IContentRegistry _registry;

        public AssetTagWriter(IContentRegistry registry)
        {
            _registry = registry;
        }

        public IEnumerable<HtmlTag> Write(IEnumerable<string> assetNames)
        {
            return assetNames.Select(x =>
            {
                // TODO -- is it possible that we could have something besides JavaScript?
                var scriptUrl = _registry.ScriptUrl(x, false);
                return new HtmlTag("script").Attr("src", scriptUrl).Attr("type", "text/javascript");
            });
        }


    }
}