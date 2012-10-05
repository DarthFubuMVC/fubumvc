using FubuTestingSupport;
using NUnit.Framework;

namespace ViewEngineIntegrationTesting.ViewEngines.Razor.HelloRazor
{
    [TestFixture]
    public class RazorEngineIntegrationTester : SharedHarnessContext
    {
        [Test]
        public void simple_get_of_razor_view_with_no_template()
        {
            var text = endpoints.Get<HelloRazorEndpoints>(x => x.SayHello(new HelloWorldRazorInputModel()))
                .ReadAsText();

            text.ShouldContain("Hello World! FubuMVC + Razor");

            // Partial text
            text.ShouldContain("<p>I'm from a partial</p>");
        }
    }
}