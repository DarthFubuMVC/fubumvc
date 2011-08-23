using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetTagPlanCacheTester : InteractionContext<AssetTagPlanCache>
    {
        [Test]
        public void only_requests_the_plan_once()
        {
            var key1 = new AssetPlanKey(MimeType.Javascript, new string[]{"a.js", "b.js", "c.js"});
            var key2 = new AssetPlanKey(MimeType.Javascript, new string[]{"d.js", "b.js", "c.js"});
            var key3 = new AssetPlanKey(MimeType.Javascript, new string[]{"e.js", "b.js", "c.js"});

            var otherKey = new AssetPlanKey(MimeType.Javascript, key1.Names);
            otherKey.ShouldEqual(key1);

            var plan1 = new AssetTagPlan(MimeType.Javascript);
            var plan2 = new AssetTagPlan(MimeType.Javascript);
            var plan3 = new AssetTagPlan(MimeType.Javascript);

            var planner = MockFor<IAssetTagPlanner>();

            planner.Expect(x => x.BuildPlan(key1)).Return(plan1).Repeat.Once();
            planner.Expect(x => x.BuildPlan(key2)).Return(plan2).Repeat.Once();
            planner.Expect(x => x.BuildPlan(key3)).Return(plan3).Repeat.Once();

            ClassUnderTest.PlanFor(key1.MimeType, key1.Names).ShouldBeTheSameAs(plan1);
            ClassUnderTest.PlanFor(key1.MimeType, key1.Names).ShouldBeTheSameAs(plan1);
            ClassUnderTest.PlanFor(key1.MimeType, key1.Names).ShouldBeTheSameAs(plan1);

            ClassUnderTest.PlanFor(key2.MimeType, key2.Names).ShouldBeTheSameAs(plan2);
            ClassUnderTest.PlanFor(key2.MimeType, key2.Names).ShouldBeTheSameAs(plan2);
            ClassUnderTest.PlanFor(key2.MimeType, key2.Names).ShouldBeTheSameAs(plan2);
            
            ClassUnderTest.PlanFor(key3.MimeType, key3.Names).ShouldBeTheSameAs(plan3);
            ClassUnderTest.PlanFor(key3.MimeType, key3.Names).ShouldBeTheSameAs(plan3);
            ClassUnderTest.PlanFor(key3.MimeType, key3.Names).ShouldBeTheSameAs(plan3);

            planner.VerifyAllExpectations();
        }
    }
}