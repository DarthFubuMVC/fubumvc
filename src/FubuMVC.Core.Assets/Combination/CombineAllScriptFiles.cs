using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Combination
{
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
}