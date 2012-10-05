using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Content
{
    public interface ITransformerPolicy
    {
        IEnumerable<string> Extensions { get; }
        ActionType ActionType { get; }
        Type TransformerType { get; }
        MimeType MimeType { get; }
        int? MatchingExtensionPosition(IList<string> extensions);
        bool AppliesTo(AssetFile file);
        bool MustBeAfter(ITransformerPolicy policy);
        bool MustBeBatched();
    }
}