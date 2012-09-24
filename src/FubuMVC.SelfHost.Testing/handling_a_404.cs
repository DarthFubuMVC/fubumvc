using System.Net;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.SelfHost.Testing
{
    [TestFixture]
    public class handling_a_404
    {
        [Test]
        public void get_response_for_non_existent_route()
        {
            var response = SelfHostHarness.Endpoints.Get(SelfHostHarness.Root + "/nonexistent");
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }
    }
}