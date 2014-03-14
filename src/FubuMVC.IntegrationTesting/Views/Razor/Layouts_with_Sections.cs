using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Razor
{
    [TestFixture]
    public class Layouts_with_Sections : ViewIntegrationContext
    {
        public Layouts_with_Sections()
        {
            RazorView<HasLayoutsWithSectionModel>("HasLayoutWithSections").Write(@"
<h1>@Model.Message</h1>
<p>HasLayoutWithSections.cshtml</p>
@section header
{
    <header>Header from template.</header>
}
");

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
        }

        [Test]
        public void get_views_with_optional_sections()
        {
            Scenario.Get.Action<HasLayoutsWithSectionsEndpoint>(x => x.get_razor_layouts_with_sections());

            Scenario.ContentShouldContain("<header>Header from template.</header>");
            Scenario.ContentShouldContain("<p>HasLayoutWithSections.cshtml</p>");
        }
    }

    public class HasLayoutsWithSectionsEndpoint
    {
        public HasLayoutsWithSectionModel get_razor_layouts_with_sections()
        {
            return new HasLayoutsWithSectionModel { Message = "Hello from endpoint" };
        }
    }


    public class HasLayoutsWithSectionModel
    {
        public string Message { get; set; }
    }
}