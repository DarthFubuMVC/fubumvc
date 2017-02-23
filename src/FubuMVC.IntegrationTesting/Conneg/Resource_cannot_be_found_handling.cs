using System.Net;
using Xunit;

namespace FubuMVC.IntegrationTesting.Conneg
{
    
    public class Resource_cannot_be_found_handling 
    {
        [Fact]
        public void the_resource_not_found_handler_should_kick_in()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<MissingResourceEndpoint>(x => x.get_missing());
                _.StatusCodeShouldBe(HttpStatusCode.NotFound);
            });
        }
    }

    public class MissingResourceEndpoint
    {
        public MissingResource get_missing()
        {
            return null;
        }
    }

    public class MissingResource
    {
    }
}