using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Transformation
{
    public interface ITransformContext
    {
        
    }

    public interface IContentSource
    {
        string GetContent(ITransformContext context);
        IEnumerable<AssetFile> Files { get; }
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

    public enum ActionType
    {
        Generate,
        Substitution,
        Transformation,
        BatchedTransformation, 
        Global // minification mostly, but might use this for tracing too
    }

    public class TransformationComparer : IComparer<ITransformationPolicy>
    {
        private readonly TransformationPolicyLibrary _library;

        public TransformationComparer(TransformationPolicyLibrary library)
        {
            _library = library;
        }

        public int Compare(ITransformationPolicy x, ITransformationPolicy y)
        {
            throw new NotImplementedException();
        }
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

    
    public class TransformationPolicyLibrary
    {
        // has all the TransformationPolicy's
        // TODO -- this thing needs to play in the bootstrapping to get the mimetypes
    }

    public interface ITransformationPolicy
    {
        IEnumerable<string> Extensions { get; }
        ActionType ActionType { get; }
        Type TransformerType { get; }
        MimeType MimeType { get; }
        int? MatchingExtensionPosition(IList<string> extensions);
        bool AppliesTo(AssetFile file);
        bool MustBeAfter(ITransformationPolicy policy);
    }
}