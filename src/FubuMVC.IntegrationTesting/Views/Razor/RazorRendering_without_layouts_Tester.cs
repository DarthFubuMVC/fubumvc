using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Razor
{
    [TestFixture]
    public class RazorRendering_without_layouts_Tester : ViewIntegrationContext
    {
        public RazorRendering_without_layouts_Tester()
        {
            RazorView<HelloWorldRazorViewModel>("HelloWorld")
                .WriteLine("<h1>@Model.Message</h1>")
                .WriteLine("@this.Partial(new HelloRazorPartialInputModel())");
        }

        [Test]
        public void renders_a_simple_razor_view()
        {
            Scenario.Get.Action<HelloRazorEndpoints>(e => e.get_razor_hello());

            Scenario.ContentShouldContain("Hello World! FubuMVC + Razor");
        }

        [Test]
        public void can_render_partials()
        {
            Scenario.Get.Action<HelloRazorEndpoints>(e => e.get_razor_hello());

            Scenario.ContentShouldContain("Hello from Partial");
        }
    }

    public class HelloRazorEndpoints
    {
        public HelloWorldRazorViewModel get_razor_hello()
        {
            return new HelloWorldRazorViewModel {Message = "Hello World! FubuMVC + Razor"};
        }

        public HelloRazorPartialViewModel SayHelloPartial(HelloRazorPartialInputModel input)
        {
            return new HelloRazorPartialViewModel {Message = "Hello from Partial"};
        }
    }

    public class HelloRazorPartialViewModel
    {
        public string Message { get; set; }
    }

    public class HelloRazorPartialInputModel
    {
    }

    public class HelloWorldRazorInputModel
    {
    }

    public class HelloWorldRazorViewModel
    {
        public string Message { get; set; }
    }
}