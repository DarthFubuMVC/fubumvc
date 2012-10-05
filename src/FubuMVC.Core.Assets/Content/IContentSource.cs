using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Content
{
    public interface IContentSource
    {
        IEnumerable<AssetFile> Files { get; }

        IEnumerable<IContentSource> InnerSources { get; }
        string GetContent(IContentPipeline pipeline);
    }


}