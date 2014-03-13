using System.Web;
using FubuMVC.Core.Http.AspNet;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http.AspNet
{

    [TestFixture]
    public class AspNetCurrentHttpRequestTester : InteractionContext<AspNetHttpRequest>
    {
        [Test]
        public void full_url_should_return_Request_Url_property_value()
        {
            var request = MockRepository.GenerateStub<HttpRequestBase>();
            var expectedUri = "https://foo.bar:999/baz?x=y";
            request.Stub(r => r.Url).Return(new System.Uri(expectedUri));
            var currentRequest = new AspNetHttpRequest(request, MockFor<HttpResponseBase>());
            currentRequest.FullUrl().ShouldEqual(expectedUri);
        }
    }
}