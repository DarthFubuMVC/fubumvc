using System;
using System.Collections.Generic;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Combination
{
    public interface ICombinationPolicy
    {
        MimeType MimeType { get; }
        IEnumerable<AssetFileCombination> DetermineCombinations(AssetTagPlan plan);
    }
}