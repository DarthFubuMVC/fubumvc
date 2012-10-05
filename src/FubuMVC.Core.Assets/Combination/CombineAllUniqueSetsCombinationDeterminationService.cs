using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Assets.Combination
{
    /// <summary>
    /// Simplistic combination that creates a combination for every unique set of scripts or styles.
    /// It does not try to cross mimetype boundaries.
    /// </summary>
    public class CombineAllUniqueSetsCombinationDeterminationService : CombinationDeterminationService
    {
        // TODO -- this is going to have to be hit with integrated tests

        public CombineAllUniqueSetsCombinationDeterminationService(IAssetCombinationCache combinations)
            : base(combinations, new List<ICombinationPolicy> { new CombineAllScriptFiles(), new CombineAllStylesheets() })
        {
        }

        public override void TryToReplaceWithCombinations(AssetTagPlan plan)
        {
            var mimeTypePolicies = policies.Where(x => x.MimeType == plan.MimeType);
            mimeTypePolicies.Each(p => ExecutePolicy(plan, p));
            base.TryToReplaceWithCombinations(plan);
        }
    }
}