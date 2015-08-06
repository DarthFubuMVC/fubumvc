using System.Net;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class handling_a_404
    {
        [Test]
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