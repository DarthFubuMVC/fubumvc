using System.Collections.Generic;

namespace FubuMVC.Core.Assets.Combination
{
    /// <summary>
    /// Simplistic combination just creates a combination for ever unique set of scripts or styles
    /// </summary>
    public class CombineAllUniqueSetsCombinationDeterminationService : CombinationDeterminationService
    {
        // TODO -- this is going to have to be hit with integrated tests

        public CombineAllUniqueSetsCombinationDeterminationService(IAssetCombinationCache combinations)
            : base(combinations, new List<ICombinationPolicy>() { new CombineAllScriptFiles(), new CombineAllStylesheets() })
        {
        }

        public override void TryToReplaceWithCombinations(AssetTagPlan plan)
        {
            policies.Each(p => ExecutePolicy(plan, p));
            base.TryToReplaceWithCombinations(plan);
        }
    }
}