using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Razor
{
    [TestFixture]
    public class Do_not_use_layout_if_view_is_being_rendered_as_partial : ViewIntegrationContext
    {
        public Do_not_use_layout_if_view_is_being_rendered_as_partial()
        {
            RazorView("Shared/Application").Write(@"
<!DOCTYPE html>
<html>
    <head>
        <title>Test title</title>
    </head>
    <body>
        @RenderSection('header', false)
        <h2>Application.cshtml</h2>
        @RenderBody()
        @RenderSection('footer', false)
    </body>
</html>
");
            RazorView("Shared/Chrome").Write(@"
@layout none
<h1>Text from Chrome</h1>
@RenderBody()
");




            RazorView<ActionlessView1>("View1").Write(@"
@layout Chrome
<p>I am in view 1</p>
");

            RazorView<ActionlessView2>("View2").Write(@"
<p>I am in view 2</p>

@this.Partial(new FubuMVC.IntegrationTesting.Views.ActionlessView1())

");

        }

        [Test]
        public void rendering_as_the_main_view_gets_the_layout()
        {
            Scenario.Get.Input<ActionlessView1>();

            Scenario.ContentShouldContain("I am in view 1");
            Scenario.ContentShouldContain("<h1>Text from Chrome</h1>");
        }

        [Test]
        public void use_the_same_view_as_a_partial_and_it_does_not_use_the_layout()
        {
            Scenario.Get.Input<ActionlessView2>();

            Scenario.ContentShouldContain("I am in view 1");
            Scenario.ContentShouldContain("I am in view 2");
            Scenario.ContentShouldNotContain("<h1>Text from Chrome</h1>");
        }
    }

    
}