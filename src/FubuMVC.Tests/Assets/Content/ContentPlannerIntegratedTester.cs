using NUnit.Framework;

namespace FubuMVC.Tests.Assets.Content
{
    [TestFixture]
    public class ContentPlannerIntegratedTester
    {
        [Test]
        public void build_a_plan_for_a_single_file_with_no_transformers_of_any_kind()
        {
            ContentPlanScenario.For(x =>
            {
                x.SingleAssetFileName = "script1.js";
            })
            .ShouldMatch(@"
FileRead:script1.js
");
        }
    }
}