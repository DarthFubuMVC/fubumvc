using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using HtmlTags;

namespace FubuMVC.Core.Assets.Tags
{
    public class TraceOnlyMissingAssetHandler : IMissingAssetHandler
    {
        private readonly ICurrentHttpRequest _httpRequest;

        public TraceOnlyMissingAssetHandler(ICurrentHttpRequest httpRequest)
        {
            _httpRequest = httpRequest;
        }

        // TODO -- trace here!!!
        public IEnumerable<HtmlTag> BuildTagsAndRecord(IEnumerable<MissingAssetTagSubject> subjects)
        {
            return subjects.Select(s =>
            {
                var url = "missing/assets/" + s.Name;
                return new HtmlTag("script")
                    .Attr("type", MimeType.Javascript.Value)
                    .Attr("src", _httpRequest.ToFullUrl(url));
            });
        }
    }
}