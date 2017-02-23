using System.Web;
using FubuMVC.Core.Http.AspNet;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http.AspNet
{

    
    public class AspNetCurrentHttpRequestTester : InteractionContext<AspNetHttpRequest>
    {
        [Fact]
        public void full_url_should_return_Request_Url_property_value()
        {
            var request = MockRepository.GenerateStub<HttpRequestBase>();
            var expectedUri = "https://foo.bar:999/baz?x=y";
            request.Stub(r => r.Url).Return(new System.Uri(expectedUri));
            var currentRequest = new AspNetHttpRequest(request, MockFor<HttpResponseBase>());
            currentRequest.FullUrl().ShouldBe(expectedUri);
        }
    }
}