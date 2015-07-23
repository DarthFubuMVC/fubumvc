using System.Net;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Security.Authentication
{
    [TestFixture]
    public class unauthenticated_request_against_an_unauthenticated_route : AuthenticationHarness
    {
        [Test]
        public void nothing_happens()
        {
            var model = new PublicModel {Message = "Test"};
            var response = endpoints.GetByInput(model, configure: r => r.AllowAutoRedirect = false);
            
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.ReadAsText().ShouldBe(model.Message);
        }
    }
}