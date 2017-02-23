using System.Net;
using Shouldly;
using Xunit;

namespace FubuMVC.IntegrationTesting.Owin
{
    
    public class handling_a_404
    {
        [Fact]
        public void get_response_for_non_existent_route()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Url("nonexistent");
                _.StatusCodeShouldBe(HttpStatusCode.NotFound);
            });
        }
    }
}