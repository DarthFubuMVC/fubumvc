using Fubu.Running;
using FubuTestingSupport;
using NUnit.Framework;

namespace fubu.Testing.Running
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