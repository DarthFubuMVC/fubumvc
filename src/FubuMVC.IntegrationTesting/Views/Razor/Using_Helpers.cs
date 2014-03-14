using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Razor
{
    [TestFixture]
    public class Using_Helpers : ViewIntegrationContext
    {
        public Using_Helpers()
        {
            RazorView<UsingHelpersWithAttrModel>("UsingHelpersWithAttr").Write(@"

<h1>@Model.Message</h1>
<p>UsingHelpersWithAttr.cshtml</p>
@helper helperWithAttrWriting(string msg)
    {
    <div>
    @if (true)
    {
        <div>
            @if (true)
            {
                <a href='@msg' class='@msg test'>rendered by helper: @msg</a>
            }
        </div>
    }
    </div>
}
@helperWithAttrWriting('helperWithAttribute')


");

            RazorView<UsingHelpersModel>("UsingHelpers").Write(@"

<h1>@Model.Message</h1>
<p>HasLayoutWithSections.cshtml</p>
@helper helperTest(string msg)
{
    <p>rendered by helper: @msg</p>
}
@helperTest('helper message')

");


        }

        [Test]
        public void get_view_with_helper_render()
        {
            Scenario.Get.Action<UsingHelpersEndpoint>(x => x.get_razor_using_helpers());

            Scenario.ContentShouldContain("<p>rendered by helper: helper message</p>");
        }

        [Test]
        public void get_view_with_attrHelper_render()
        {
            Scenario.Get.Action<UsingHelpersWithAttrEndpoint>(x => x.get_razor_using_helpers_attr());

            Scenario.ContentShouldContain("<a href=\"helperWithAttribute\" class=\"helperWithAttribute test\">rendered by helper: helperWithAttribute</a>");
        }
    }

    public class UsingHelpersWithAttrEndpoint
    {
        public UsingHelpersWithAttrModel get_razor_using_helpers_attr()
        {
            return new UsingHelpersWithAttrModel { Message = "Hello from UsingHelpersWithAttrModel" };
        }
    }

    public class UsingHelpersWithAttrInput
    {
    }

    public class UsingHelpersWithAttrModel
    {
        public string Message { get; set; }
    }

    public class UsingHelpersEndpoint
    {
        public UsingHelpersModel get_razor_using_helpers()
        {
            return new UsingHelpersModel { Message = "Hello from endpoint" };
        }
    }

    public class UsingHelpersInput
    {
    }

    public class UsingHelpersModel
    {
        public string Message { get; set; }
    }
}