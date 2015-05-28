using System.Net;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Authentication.IntegrationTesting
{
    [TestFixture]
    public class unauthenticated_request_against_an_unauthenticated_route : AuthenticationHarness
    {
        [Test]
        public void nothing_happens()
        {
            var model = new PublicModel {Message = "Test"};
            var response = endpoints.GetByInput(model, configure: r => r.AllowAutoRedirect = false);
            
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            response.ReadAsText().ShouldEqual(model.Message);
        }
    }
}