using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Assets.Combination
{
    public class CombinationDeterminationService : ICombinationDeterminationService
    {
        private readonly AssetGraph _graph;
        private readonly IAssetCombinationCache _cache;
        private readonly IEnumerable<ICombinationPolicy> _policies;

        public CombinationDeterminationService(AssetGraph graph, IAssetCombinationCache cache, IEnumerable<ICombinationPolicy> policies)
        {
            _graph = graph;
            _cache = cache;
            _policies = policies;
        }

        public void TryToReplaceWithCombinations(AssetTagPlan plan)
        {
            tryAllExistingCombinations(plan);

            tryCombinationCandidatesAndPolicies(plan);
        }

        private void tryCombinationCandidatesAndPolicies(AssetTagPlan plan)
        {
            throw new NotImplementedException();

            // will need to depend on CombinationCandidateCache here.

            //var mimeTypePolicies = _policies.Where(x => x.MimeType == plan.MimeType);
            //var combinationPolicies = _graph.OrderedCombinationCandidates(plan.MimeType).Union(mimeTypePolicies);
            //combinationPolicies.Each(policy => ExecutePolicy(plan, policy));
        }

        public void ExecutePolicy(AssetTagPlan plan, ICombinationPolicy policy)
        {
            throw new NotImplementedException();
        }

        private void tryAllExistingCombinations(AssetTagPlan plan)
        {
            _cache.OrderedListOfCombinations(plan.MimeType).Each(combo => plan.TryCombination(combo));
        }
    }
}