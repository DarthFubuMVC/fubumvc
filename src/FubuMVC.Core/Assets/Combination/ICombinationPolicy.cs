using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using System.Linq;

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
            var grouper = new AssetGrouper<IAssetTagSubject>();
            var groups = grouper.GroupSubjects(plan.Subjects, s => s is AssetFile)
                .Where(x => x.Count > 1);

            foreach (var @group in groups)
            {
                yield return new ScriptFileCombination(@group.Cast<AssetFile>().ToList());
            }
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