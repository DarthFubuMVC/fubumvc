using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Content
{
    [TestFixture]
    public class ContentPlanCacheTester : InteractionContext<ContentPlanCache>
    {
        [Test]
        public void caches_the_content_plan_from_the_planner()
        {
            var jsFile = new AssetFile("script.js"){
                FullPath = "something"
            };

            var thePlan = new ContentPlan("something", new AssetFile[]{jsFile});

            MockFor<IContentPlanner>().Expect(x => x.BuildPlanFor(thePlan.Name))
                .Return(thePlan)
                .Repeat.Once();

            ClassUnderTest.PlanFor(thePlan.Name).ShouldBeTheSameAs(thePlan);
            ClassUnderTest.PlanFor(thePlan.Name).ShouldBeTheSameAs(thePlan);
            ClassUnderTest.PlanFor(thePlan.Name).ShouldBeTheSameAs(thePlan);
            ClassUnderTest.PlanFor(thePlan.Name).ShouldBeTheSameAs(thePlan);
            ClassUnderTest.PlanFor(thePlan.Name).ShouldBeTheSameAs(thePlan);
            ClassUnderTest.PlanFor(thePlan.Name).ShouldBeTheSameAs(thePlan);

            MockFor<IContentPlanner>().VerifyAllExpectations();
        }
    }
}