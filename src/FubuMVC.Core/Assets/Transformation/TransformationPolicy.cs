using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Assets.Transformation
{
    public class TransformationPolicy : ITransformationPolicy
    {
        private readonly ActionType _actionType;
        private readonly IList<string> _extensions = new List<string>();
        private readonly MimeType _mimeType;
        private readonly Type _transformerType;
        private readonly IList<Func<AssetFile, bool>> _matchingCriteria = new List<Func<AssetFile, bool>>();
        private readonly IList<Func<ITransformationPolicy, bool>> _mustBeAfterRules = new List<Func<ITransformationPolicy, bool>>();

        public TransformationPolicy(ActionType actionType, MimeType mimeType, Type transformerType)
        {
            if (!transformerType.IsConcreteTypeOf<IAssetTransformer>())
            {
                throw new ArgumentOutOfRangeException("Type {0} is not a concrete type of {1}".ToFormat(transformerType.FullName, typeof(IAssetTransformer).FullName));
            }

            _actionType = actionType;
            _mimeType = mimeType;
            _transformerType = transformerType;

            _matchingCriteria.Add(file =>
            {
                var fileExtensions = file.AllExtensions();
                return _extensions.Any(x => fileExtensions.Contains(x));
            });
        }

        public void AddMatchingCriteria(Func<AssetFile, bool> criteria)
        {
            _matchingCriteria.Add(criteria);
        }

        public void AddMustBeAfterRule(Func<ITransformationPolicy, bool> mustBeAfterTest)
        {
            _mustBeAfterRules.Add(mustBeAfterTest);
        }

        public void AddExtension(string extension)
        {
            _extensions.Add(extension);
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
            return _matchingCriteria.Any(x => x(file));
        }

        public bool MustBeAfter(ITransformationPolicy policy)
        {
            return _mustBeAfterRules.Any(x => x(policy));
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

    public class JavascriptTransformationPolicy<T> : TransformationPolicy where T : IAssetTransformer
    {
        public JavascriptTransformationPolicy(ActionType actionType) : base(actionType, MimeType.Javascript, typeof(T))
        {
        }

        public static JavascriptTransformationPolicy<T> For(ActionType actionType, params string[] extensions)
        {
            var policy = new JavascriptTransformationPolicy<T>(actionType);
            extensions.Each(policy.AddExtension);

            return policy;
        }
    }

    public class CssTransformationPolicy<T> : TransformationPolicy where T : IAssetTransformer
    {
        public CssTransformationPolicy(ActionType actionType)
            : base(actionType, MimeType.Css, typeof(T))
        {
        }

        public static CssTransformationPolicy<T> For(ActionType actionType, params string[] extensions)
        {
            var policy = new CssTransformationPolicy<T>(actionType);
            extensions.Each(policy.AddExtension);

            return policy;
        }
    }
}