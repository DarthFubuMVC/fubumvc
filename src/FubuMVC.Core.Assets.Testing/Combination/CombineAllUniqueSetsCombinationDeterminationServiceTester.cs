using System.Linq;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets.Combination
{
    [TestFixture]
    public class CombineAllUniqueSetsCombinationDeterminationServiceTester : InteractionContext<CombineAllUniqueSetsCombinationDeterminationService>
    {
        protected override void beforeEach()
        {
            Services.Inject<IAssetCombinationCache>(new AssetCombinationCache());
        }

        [Test]
        public void executes_correct_policy_1()
        {
            run_with_plan(new AssetTagPlan(MimeType.Javascript, new[]
            {
                new AssetFile("A.css"), new AssetFile("B.css")
            }));
        }

        [Test]
        public void executes_correct_policy_2()
        {
            run_with_plan(new AssetTagPlan(MimeType.Javascript, new[]
            {
                new AssetFile("C.js"), new AssetFile("D.js")
            }));
        }


        private void run_with_plan(AssetTagPlan plan)
        {
            ClassUnderTest.TryToReplaceWithCombinations(plan);
            
            plan.Subjects.ShouldHaveCount(1)
                .First().MimeType.ShouldEqual(plan.MimeType);
        }
    }
}