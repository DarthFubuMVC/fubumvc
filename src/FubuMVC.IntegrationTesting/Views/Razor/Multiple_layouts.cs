using System.Linq;
using FubuMVC.Razor.RazorModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Razor
{
    [TestFixture]
    public class Multiple_layouts : ViewIntegrationContext
    {
        public Multiple_layouts()
        {
            RazorView("One/Shared/Application").Write(@"
@layout Html
<h2>One/Shared/Application.cshtml</h2>
<div id='page'>
    @RenderBody()
</div>
");

            RazorView<ClosestViewModel>("One/Closest").WriteLine("<h1>Closest.cshtml</h1>");

            RazorView<UsesDefaultViewModel>("Two/UsesDefault/UsesDefault").WriteLine("<h1>UsesDefault.cshtml</h1>");

            RazorView("Shared/Application").Write(@"
<h2>Default Layout</h2>
<div id='page'>
    @RenderBody()
</div>
");

            RazorView("Shared/Html").Write(@"
@layout none
<!DOCTYPE html>
<html>
    <head>
        <title>Test title</title>
    </head>
    <body>
        <h2>Default Html.cshtml</h2>
        @if (this.ServiceLocator != null)
        {
            @RenderBody()
        }
    </body>
</html>
");
        }



        [Test]
        public void uses_layout_in_closest_shared_directory_when_found()
        {
//            var view = Views.Templates<RazorTemplate>().FirstOrDefault(x => x.Name() == "Closest");
//            view.Master.ViewPath.ShouldEqual("One/Shared/Application.cshtml");
//            view.Master.Master.ShouldBeNull();

            Scenario.Get.Action<ClosestEndpoint>(x => x.get_razor_closest());

            Scenario.ContentShouldContain("<h1>Closest.cshtml</h1>");
            Scenario.ContentShouldContain("<h2>One/Shared/Application.cshtml</h2>");
            Scenario.ContentShouldContain("<h2>Default Html.cshtml</h2>");

            Scenario.ContentShouldNotContain("<h2>Default Layout</h2>");
        }

        [Test]
        public void uses_default_layout_when_none_are_found_in_closer_directory()
        {
            Scenario.Get.Action<UsesDefaultEndpoint>(x => x.get_razor_uses_default());

            Scenario.ContentShouldContain("<h1>UsesDefault.cshtml</h1>");
            Scenario.ContentShouldContain("<h2>Default Layout</h2>");
        }

    }

    public class ClosestEndpoint
    {
        public ClosestViewModel get_razor_closest()
        {
            return new ClosestViewModel();
        }
    }

    public class ClosestViewModel
    {
    }

    public class UsesDefaultEndpoint
    {
        public UsesDefaultViewModel get_razor_uses_default()
        {
            return new UsesDefaultViewModel();
        }
    }

    public class UsesDefaultViewModel
    {
    }
}