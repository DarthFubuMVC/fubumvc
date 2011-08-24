using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Content
{
    public interface ITransformContext
    {
        string ReadContentsFrom(string file);
    }

    public interface IContentSource
    {
        string GetContent(ITransformContext context);
        IEnumerable<AssetFile> Files { get; }
    }

    public class TransformSource<T> : IContentSource where T : IAssetTransformer
    {
        private readonly IContentSource _inner;

        public TransformSource(IContentSource inner)
        {
            _inner = inner;
        }

        public string GetContent(ITransformContext context)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AssetFile> Files
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class CombiningContentSource : IContentSource
    {
        private readonly IEnumerable<IContentSource> _innerSources;

        public CombiningContentSource(IEnumerable<IContentSource> innerSources)
        {
            _innerSources = innerSources;
        }

        public string GetContent(ITransformContext context)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AssetFile> Files
        {
            get { throw new NotImplementedException(); }
        }
    }

    public interface IAssetTransformer
    {
        string Transform(IContentSource inner);
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