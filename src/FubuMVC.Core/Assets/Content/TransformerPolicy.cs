using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
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

        public GlobalTransformerPolicy(MimeType mimeType, BatchBehavior batching)
            : base(ActionType.Global, mimeType, typeof (T))
        {
            _batching = batching;
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
        private readonly IList<Func<AssetFile, bool>> _matchingCriteria = new List<Func<AssetFile, bool>>();
        private readonly MimeType _mimeType;

        private readonly IList<Func<ITransformerPolicy, bool>> _mustBeAfterRules =
            new List<Func<ITransformerPolicy, bool>>();

        private readonly Type _transformerType;

        public TransformerPolicy(ActionType actionType, MimeType mimeType, Type transformerType)
        {
            if (!transformerType.IsConcreteTypeOf<ITransformer>())
            {
                throw new ArgumentOutOfRangeException(
                    "Type {0} is not a concrete type of {1}".ToFormat(transformerType.FullName,
                                                                      typeof (ITransformer).FullName));
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

        public bool MustBeAfter(ITransformerPolicy policy)
        {
            return _mustBeAfterRules.Any(x => x(policy));
        }

        public virtual bool MustBeBatched()
        {
            return ActionType == ActionType.BatchedTransformation;
        }

        public void AddMatchingCriteria(Func<AssetFile, bool> criteria)
        {
            _matchingCriteria.Add(criteria);
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