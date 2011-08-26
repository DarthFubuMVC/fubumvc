using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Combination
{
    public interface ICombinationPolicy
    {
        MimeType MimeType { get; }
        IEnumerable<AssetFileCombination> DetermineCombinations(AssetTagPlan plan);
    }

    public class CombineAllScriptFiles : ICombinationPolicy
    {
        public MimeType MimeType
        {
            get { return MimeType.Javascript; }
        }

        public IEnumerable<AssetFileCombination> DetermineCombinations(AssetTagPlan plan)
        {
            throw new NotImplementedException();
        }
    }


    public class CombineAllStylesheets : ICombinationPolicy
    {
        public MimeType MimeType
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<AssetFileCombination> DetermineCombinations(AssetTagPlan plan)
        {
            throw new NotImplementedException();
        }
    }
}