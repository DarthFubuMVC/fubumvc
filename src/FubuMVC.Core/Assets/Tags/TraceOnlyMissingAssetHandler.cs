using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime;
using HtmlTags;

namespace FubuMVC.Core.Assets.Tags
{
    public class TraceOnlyMissingAssetHandler : IMissingAssetHandler
    {
        // TODO -- trace here!!!
        public IEnumerable<HtmlTag> BuildTagsAndRecord(IEnumerable<MissingAssetTagSubject> subjects)
        {
            throw new NotImplementedException();
            //return subjects.Select(s =>
            //{
            //    var url = "missing/assets/" + s.Name;
            //    return new HtmlTag("script")
            //        .Attr("type", MimeType.Javascript.Value)
            //        .Attr("src", url.ToAbsoluteUrl());
            //});
        }
    }
}