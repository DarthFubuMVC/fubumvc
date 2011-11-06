using FubuMVC.Core.Assets.Combination;

namespace Serenity.Jasmine
{
    public class NulloCombinationDeterminationService : ICombinationDeterminationService
    {
        public void TryToReplaceWithCombinations(AssetTagPlan plan)
        {
            // That's right, do nothing
        }
    }
}