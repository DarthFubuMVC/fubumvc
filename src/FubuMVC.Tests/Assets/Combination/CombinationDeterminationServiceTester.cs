using System;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;

namespace FubuMVC.Tests.Assets.Combination
{
    [TestFixture]
    public class CombinationDeterminationServiceTester : InteractionContext<CombinationDeterminationService>
    {
        [Test]
        public void execute_a_combination_policy()
        {
            var policy = MockFor<ICombinationPolicy>();
            var combos = new AssetFileCombination[]{
                new ScriptFileCombination(new AssetFile[0]),
                new ScriptFileCombination(new AssetFile[0]),
                new ScriptFileCombination(new AssetFile[0]),
                new ScriptFileCombination(new AssetFile[0])
            };



            var thePlan = MockRepository.GenerateMock<AssetTagPlan>(MimeType.Css, new AssetFile[0]);

            // Leaving this in here.  What I found out in the Storyteller testing is that
            // we have to apply the combination policy "go find existing combinations" first
            //combos.Each(combo => thePlan.Expect(x => x.TryCombination(combo)).Return(true));

            policy.Stub(x => x.DetermineCombinations(thePlan)).Return(combos);

            ClassUnderTest.ExecutePolicy(thePlan, policy);

            combos.Each(c => MockFor<IAssetCombinationCache>().AssertWasCalled(x => x.StoreCombination(thePlan.MimeType ,c)));

            // All the combos should have been registered
            thePlan.VerifyAllExpectations();
        }

        [Test]
        public void try_all_existing_combinations()
        {
            var combos = new AssetFileCombination[]{
                new ScriptFileCombination(new AssetFile[0]),
                new ScriptFileCombination(new AssetFile[0]),
                new ScriptFileCombination(new AssetFile[0]),
                new ScriptFileCombination(new AssetFile[0])
            };

            var thePlan = MockRepository.GenerateMock<AssetTagPlan>(MimeType.Css, new AssetFile[0]);
            MockFor<IAssetCombinationCache>().Stub(x => x.OrderedListOfCombinations(thePlan.MimeType))
                .Return(combos);

            
            combos.Each(c => thePlan.Expect(x => x.TryCombination(c)).Return(true));

            ClassUnderTest.TryAllExistingCombinations(thePlan);

            // Should try all the existing combinations 
            thePlan.VerifyAllExpectations();
        }
    }

    public class RecordingCombinationDeterminationService : CombinationDeterminationService
    {
        public RecordingCombinationDeterminationService(AssetGraph graph, IAssetCombinationCache cache, IEnumerable<ICombinationPolicy> policies) : base(cache, policies)
        {
        }

        public IList<ICombinationPolicy> Policies = new List<ICombinationPolicy>();

        public override void ExecutePolicy(AssetTagPlan plan, ICombinationPolicy policy)
        {
            Policies.Add(policy);
        }
    }

    [TestFixture]
    public class when_trying_all_combination_candidates_and_policies_for_an_AssetTagPlan : InteractionContext<RecordingCombinationDeterminationService>
    {
        private List<CombinationCandidate> theCandidates;
        private ICombinationPolicy[] thePolicies;

        protected override void beforeEach()
        {
            theCandidates = new List<CombinationCandidate>{
                new CombinationCandidate(MimeType.Css, "a"),
                new CombinationCandidate(MimeType.Css, "b"),
                new CombinationCandidate(MimeType.Css, "c"),
                new CombinationCandidate(MimeType.Css, "d"),
                new CombinationCandidate(MimeType.Css, "e")
            };

            MockFor<IAssetCombinationCache>().Stub(x => x.OrderedListOfCombinations(MimeType.Css))
                .Return(new AssetFileCombination[0]);

            MockFor<IAssetCombinationCache>().Stub(x => x.OrderedCombinationCandidatesFor(MimeType.Css))
                .Return(theCandidates);

            thePolicies = Services.CreateMockArrayFor<ICombinationPolicy>(4);
            thePolicies.Each(p => p.Stub(x => x.MimeType).Return(MimeType.Css));

            ClassUnderTest.TryToReplaceWithCombinations(new AssetTagPlan(MimeType.Css, new AssetFile[0]));
        }

        [Test]
        public void ran_all_the_candidates_and_policies_in_correct_order()
        {
            // The candidates should run first in the order that comes out of
            // IAssetCombinationCache
            ClassUnderTest.Policies[0].ShouldBeTheSameAs(theCandidates[0]);
            ClassUnderTest.Policies[1].ShouldBeTheSameAs(theCandidates[1]);
            ClassUnderTest.Policies[2].ShouldBeTheSameAs(theCandidates[2]);
            ClassUnderTest.Policies[3].ShouldBeTheSameAs(theCandidates[3]);
            ClassUnderTest.Policies[4].ShouldBeTheSameAs(theCandidates[4]);


            ClassUnderTest.Policies[5].ShouldBeTheSameAs(thePolicies[0]);
            ClassUnderTest.Policies[6].ShouldBeTheSameAs(thePolicies[1]);
            ClassUnderTest.Policies[7].ShouldBeTheSameAs(thePolicies[2]);
            ClassUnderTest.Policies[8].ShouldBeTheSameAs(thePolicies[3]);
        }
    }
}