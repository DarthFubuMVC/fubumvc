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

        [Test]
        public void use_full_name_if_there_is_a_package()
        {
            var path = new AssetPath("pak1", "a.js", AssetFolder.scripts);
            var thePlan = new ContentPlan("something", new AssetFile[]{new AssetFile("a.js"){FullPath = "a.js"}, });

            MockFor<IContentPlanner>().Stub(x => x.BuildPlanFor(path.ToFullName()))
                .Return(thePlan);

            ClassUnderTest.SourceFor(path).ShouldBeTheSameAs(thePlan);
        }

        [Test]
        public void only_use_the_name_if_there_is_no_package()
        {
            var path = new AssetPath("scripts/a.js");
            var thePlan = new ContentPlan("something", new AssetFile[] { new AssetFile("a.js") { FullPath = "a.js" }, });


            MockFor<IContentPlanner>().Stub(x => x.BuildPlanFor(path.Name))
                .Return(thePlan);

            ClassUnderTest.SourceFor(path).ShouldBeTheSameAs(thePlan); 
        }
    }
}