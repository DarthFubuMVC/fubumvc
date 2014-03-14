using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Razor
{
    [TestFixture]
    public class Context_is_usable : ViewIntegrationContext
    {
        public Context_is_usable()
        {
            RazorView<ContextTestModel>("ContextTestView").Write(@"
<h1>@Model.Message</h1>
<p>ContextTestView.cshtml</p>
context: @(Context != null)
");
        }

        [Test]
        public void get_view_with_heler_render()
        {
            Scenario.Get.Action<ContextTestEndpoint>(x => x.get_razor_context());
            Scenario.ContentShouldContain("context: True");
        }
    }

    public class ContextTestEndpoint
    {
        public ContextTestModel get_razor_context()
        {
            return new ContextTestModel { Message = "Hello from endpoint" };
        }
    }


    public class ContextTestModel
    {
        public string Message { get; set; }
    }
}