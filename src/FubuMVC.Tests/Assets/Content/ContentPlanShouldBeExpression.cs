using FubuMVC.Core.Assets.Content;

namespace FubuMVC.Tests.Assets.Content
{
    public class ContentPlanShouldBeExpression
    {
        private readonly ContentPlan _plan;

        public ContentPlanShouldBeExpression(ContentPlan plan)
        {
            _plan = plan;
        }

        public void ShouldMatch(string text)
        {
            var expectation = new ContentPlanExpectation(text);
            expectation.AssertMatches(_plan);
        }
    }
}