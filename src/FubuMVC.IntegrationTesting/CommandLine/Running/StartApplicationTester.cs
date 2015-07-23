using Fubu.Running;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.CommandLine.Running
{
    [TestFixture]
    public class StartApplicationTester
    {
        [Test]
        public void use_web_sockets_address_sets_the_injection_text()
        {
            var start = new StartApplication();
            start.AutoRefreshWebSocketsAddress = "ws:foo";

            start.HtmlHeadInjectedText.ShouldContain("ws:foo");
            start.HtmlHeadInjectedText.ShouldContain("<script");
            start.HtmlHeadInjectedText.ShouldContain("</script>");
        }
    }
}