using System.Collections.Generic;
using FubuMVC.Core.Assets.Combination;
using HtmlTags;

namespace FubuMVC.Core.Assets.Tags
{
    public interface IAssetTagBuilder
    {
        IEnumerable<HtmlTag> Build(AssetTagPlan plan);
    }
}