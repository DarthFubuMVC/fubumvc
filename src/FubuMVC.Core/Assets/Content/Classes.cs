using System;
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
    }

    public interface IAssetTransformer
    {
        string Transform(string contents, IEnumerable<AssetFile> files);
    }

    public class TransformationPlan
    {
        private readonly string _name;
        private readonly IList<IContentSource> _sources = new List<IContentSource>();

        public TransformationPlan(string name, IEnumerable<AssetFile> files)
        {
            _name = name;
        }

        public IEnumerable<IContentSource> AllSources
        {
            get
            {
                return _sources;
            }
        }

        // Methods to help replace

        public CombiningContentSource Combine(IEnumerable<IContentSource> sources)
        {
            // throw exception if they are not continuous
            throw new NotImplementedException();
        }

        public IContentSource ApplyTransform(IContentSource source, Type transformerType)
        {
            throw new NotImplementedException();
        }
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