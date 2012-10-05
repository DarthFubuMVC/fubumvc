namespace FubuMVC.Core.Assets.Combination
{
    public interface ICombinationDeterminationService
    {
        void TryToReplaceWithCombinations(AssetTagPlan plan);
    }
}