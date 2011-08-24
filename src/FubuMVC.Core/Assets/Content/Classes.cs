using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Content
{
    public interface IContentPipeline
    {
        string ReadContentsFrom(string file);
        IAssetTransformer GetTransformer<T>() where T : IAssetTransformer;
    }

    public interface IContentSource
    {
        string GetContent(IContentPipeline pipeline);
        IEnumerable<AssetFile> Files { get; }

        IEnumerable<IContentSource> InnerSources { get; }
    }

    public interface IAssetTransformer
    {
        string Transform(string contents, IEnumerable<AssetFile> files);
    }

    // Given an enumerable of AssetFile's, make the transformation plan
    public class TransformationPlanner
    {
        public TransformationPlanner()
        {
        }
    }

    public class TransformationPlanCache
    {
        // keeps transformation plan per name
        // keeps transformer 
    }
}