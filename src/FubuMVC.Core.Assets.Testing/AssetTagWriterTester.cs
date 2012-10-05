using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Tags;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetTagWriterTester : InteractionContext<AssetTagWriter>
    {
        [Test]
        public void tags_for_plan_should_pull_the_plan_from_the_cache_before_building()
        {
            var tags = new HtmlTag[0];
            var key = AssetPlanKey.For(MimeType.Javascript, "a.js");
            var thePlan = new AssetTagPlan(MimeType.Javascript);

            MockFor<IAssetTagPlanCache>().Stub(x => x.PlanFor(key)).Return(thePlan);
            MockFor<IAssetTagBuilder>().Stub(x => x.Build(thePlan)).Return(tags);

            ClassUnderTest.TagsForPlan(key).ShouldBeTheSameAs(tags);

        }
    }
}