using System.Net;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class handling_a_404
    {
        [Test]
        public void get_response_for_non_existent_route()
        {
            var response = Harness.Endpoints.Get(Harness.Root + "/nonexistent");
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }
    }
}