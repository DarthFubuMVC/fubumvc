using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Content
{
    public enum BatchBehavior
    {
        NoBatching,
        MustBeBatched
    }

    public class GlobalTransformerPolicy<T> : TransformerPolicy
    {
        private readonly BatchBehavior _batching;

        public GlobalTransformerPolicy(MimeType mimeType, BatchBehavior batching, params string[] extensions)
            : base(ActionType.Global, mimeType, typeof (T))
        {
            _batching = batching;
            extensions.Each(AddExtension);
        }

        public override bool MustBeBatched()
        {
            return _batching == BatchBehavior.MustBeBatched;
        }
    }

    public class TransformerPolicy : ITransformerPolicy
    {
        private readonly ActionType _actionType;
        private readonly IList<string> _extensions = new List<string>();

        private readonly CompositeFilter<AssetFile> _filter = new CompositeFilter<AssetFile>(); 
        private readonly IList<Func<ITransformerPolicy, bool>> _mustBeAfterRules = new List<Func<ITransformerPolicy, bool>>();

        private readonly MimeType _mimeType;
        private readonly Type _transformerType;

        public TransformerPolicy(ActionType actionType, MimeType mimeType, Type transformerType)
        {
            checkTransformerType(transformerType);

            _actionType = actionType;
            _mimeType = mimeType;
            _transformerType = transformerType;

            AddMatchingCriteria(file => hasExtensionForFile(file));
        }

        private static void checkTransformerType(Type transformerType)
        {
            if (!transformerType.IsConcreteTypeOf<ITransformer>())
            {
                var exMsg = "Type {0} is not a concrete type of {1}"
                    .ToFormat(transformerType.FullName, typeof (ITransformer).FullName);

                throw new ArgumentOutOfRangeException(exMsg);
            }
        }

        private bool hasExtensionForFile(AssetFile file)
        {
            var fileExtensions = file.AllExtensions();
            return _extensions.Any(fileExtensions.Contains);
        }

        public IEnumerable<string> Extensions
        {
            get { return _extensions; }
        }

        public virtual int? MatchingExtensionPosition(IList<string> extensions)
        {
            if (_extensions.Any())
            {
                foreach (var ext in _extensions)
                {
                    var position = extensions.IndexOf(ext);
                    if (position > -1) return position;
                }
            }

            return null;
        }

        public ActionType ActionType
        {
            get { return _actionType; }
        }

        public Type TransformerType
        {
            get { return _transformerType; }
        }

        public MimeType MimeType
        {
            get { return _mimeType; }
        }

        public bool AppliesTo(AssetFile file)
        {
            return _filter.Matches(file);
        }

        public bool MustBeAfter(ITransformerPolicy policy)
        {
            return _mustBeAfterRules.Any(x => x(policy));
        }

        public virtual bool MustBeBatched()
        {
            return ActionType == ActionType.BatchedTransformation;
        }

        public void AddMatchingCriteria(Expression<Func<AssetFile, bool>> criteria)
        {
            _filter.Includes.Add(criteria);
        }

        public void AddExclusionCriteria(Expression<Func<AssetFile, bool>> criteria)
        {
            _filter.Excludes.Add(criteria);
        }

        public void AddMustBeAfterRule(Func<ITransformerPolicy, bool> mustBeAfterTest)
        {
            _mustBeAfterRules.Add(mustBeAfterTest);
        }

        public void AddExtension(string extension)
        {
            _extensions.Add(extension);
        }

        public override string ToString()
        {
            return "Transform with {0} for {1} with extensions {2} ({3})"
                .ToFormat(
                    TransformerType.Name,
                    MimeType.Value,
                    _extensions.Join(", "),
                    ActionType
                );
        }
    }

    public class JavascriptTransformerPolicy<T> : TransformerPolicy where T : ITransformer
    {
        public JavascriptTransformerPolicy(ActionType actionType, params string[] extensions)
            : base(actionType, MimeType.Javascript, typeof (T))
        {
            extensions.Each(AddExtension);
        }

        public static JavascriptTransformerPolicy<T> For(ActionType actionType, params string[] extensions)
        {
            var policy = new JavascriptTransformerPolicy<T>(actionType);
            extensions.Each(policy.AddExtension);

            return policy;
        }
    }

    public class CssTransformerPolicy<T> : TransformerPolicy where T : ITransformer
    {
        public CssTransformerPolicy(ActionType actionType, params string[] extensions)
            : base(actionType, MimeType.Css, typeof (T))
        {
            extensions.Each(AddExtension);
        }
    }
}