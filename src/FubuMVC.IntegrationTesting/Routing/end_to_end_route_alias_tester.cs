using FubuMVC.Core;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Routing
{
    [TestFixture]
    public class end_to_end_route_alias_tester : SharedHarnessContext
    {
        [Test]
        public void call_the_aliased_route()
        {
            endpoints.Get(SelfHostHarness.Root + "/something/completely/different")
                .ReadAsText()
                .ShouldBe("Hey there");
        }
    }

    public class AliasedEndpoint
    {
        [UrlAlias("something/completely/different")]
        public string get_aliased_route()
        {
            return "Hey there";
        }
    }
}