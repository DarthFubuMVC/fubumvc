using FubuMVC.Core;
using Shouldly;
using Xunit;

namespace FubuMVC.IntegrationTesting.Routing
{
    
    public class end_to_end_route_alias_tester 
    {
        [Fact]
        public void call_the_aliased_route()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Url("/something/completely/different");
                _.ContentShouldBe("Hey there");
            });
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