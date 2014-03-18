using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Razor
{
    [TestFixture]
    public class No_layout_with_use_none : ViewIntegrationContext
    {
        public No_layout_with_use_none()
        {
            RazorView<NoLayoutModel>("View1").Write(@"
@layout none

<h1>Some stuff</h1>

");

            RazorView("Shared/Application").WriteLine("Text from the Application layout");
        }

        [Test]
        public void should_not_use_any_layout_if_layout_equals_none()
        {
            Scenario.Get.Action<NoLayoutEndpoint>(x => x.get_no_layout());

            Scenario.ContentShouldContain("<h1>Some stuff</h1>");
            Scenario.ContentShouldNotContain("Text from the Application layout");
        }
    }

    public class NoLayoutModel{}

    public class NoLayoutEndpoint
    {
        public NoLayoutModel get_no_layout()
        {
            return new NoLayoutModel();
        }
    }
}