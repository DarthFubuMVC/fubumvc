using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Razor
{
    [TestFixture]
    public class Page_extensions : ViewIntegrationContext
    {
        public Page_extensions()
        {
            RazorView<PartialModel>("Shared/_withmodel").WriteLine("<h1>With model @Model.Message</h1>");

            RazorView("Shared/_withoutmodel").WriteLine("<h1>Without model</h1>");

            RazorView<PageExtensionsViewModel>("PageExtensions").Write(@"

@Model.Message
@this.RenderPartial('withoutmodel')
@this.RenderPartial('withmodel', new FubuMVC.IntegrationTesting.Views.Razor.PartialModel{Message = 'From view'})

<div id='null' class='@null' title=''></div>
");




        }

        [Test]
        public void razor_view_renders_appropriately_including_page_extensions()
        {
            Scenario.Get.Action<PageExtensionsEndpoint>(x => x.get_razor_page_extensions());

            Scenario.ContentShouldContain("From view");
            Scenario.ContentShouldContain("From controller");
            Scenario.ContentShouldContain("With model");
            Scenario.ContentShouldContain("Without model");
            Scenario.ContentShouldContain("<div id=\"null\" title=\"\"></div>");
        } 
    }

    public class PageExtensionsEndpoint
    {
        public PageExtensionsViewModel get_razor_page_extensions()
        {
            return new PageExtensionsViewModel
            {
                Message = "From controller",
            };
        }

        public LinkToViewModel get_razor_page_extensions2()
        {
            return new LinkToViewModel();
        }
    }

    public class LinkToInputModel
    {
    }

    public class LinkToViewModel
    {
    }

    public class PageExtensionsViewModel
    {
        public string Message { get; set; }
    }

    public class PartialModel
    {
        public string Message { get; set; }
    }
}