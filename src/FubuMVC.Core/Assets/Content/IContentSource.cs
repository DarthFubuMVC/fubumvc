using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Content
{
    public interface IContentSource
    {
        string GetContent(IContentPipeline pipeline);
        IEnumerable<AssetFile> Files { get; }

        IEnumerable<IContentSource> InnerSources { get; }
    }
}