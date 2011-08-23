using System.Collections.Generic;
using HtmlTags;

namespace FubuMVC.Core.Assets.Tags
{
    public interface IMissingAssetHandler
    {
        IEnumerable<HtmlTag> BuildTagsAndRecord(IEnumerable<MissingAssetTagSubject> subjects);
    }
}