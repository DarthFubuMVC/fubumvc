using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Combination
{
    public class CombineAllStylesheets : ICombinationPolicy
    {
        public MimeType MimeType
        {
            get { return MimeType.Css; }
        }

        public IEnumerable<AssetFileCombination> DetermineCombinations(AssetTagPlan plan)
        {
            var folders = plan.Subjects.OfType<AssetFile>().Select(x => x.ContentFolder()).Distinct();
            foreach (var folder in folders)
            {
                var grouper = new AssetGrouper<IAssetTagSubject>();
                var groups = grouper.GroupSubjects(plan.Subjects, s => s is AssetFile && s.As<AssetFile>().ContentFolder() == folder)
                    .Where(x => x.Count > 1);

                foreach (var @group in groups)
                {
                    yield return new StyleFileCombination(folder, @group.Cast<AssetFile>().ToList());
                }
            }

        }
    }
}