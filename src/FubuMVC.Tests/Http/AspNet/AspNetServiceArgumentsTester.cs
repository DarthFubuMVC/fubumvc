using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using FubuCore.Binding;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.AspNet;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Http.AspNet
{
    
    public class AspNetServiceArgumentsTester
    {
        private HttpContextBase theHttpContext;
        private RequestContext theRequestContext;
        private AspNetServiceArguments theArguments;
        private HttpRequestBase theHttpRequest;
        private HttpResponseBase theHttpResponse;

        public AspNetServiceArgumentsTester()
        {
            theHttpContext = MockRepository.GenerateMock<HttpContextBase>();
            theRequestContext = new RequestContext(theHttpContext, new RouteData());

            theHttpRequest = MockRepository.GenerateMock<HttpRequestBase>();
            theHttpContext.Stub(x => x.Request).Return(theHttpRequest);

            theHttpResponse = MockRepository.GenerateMock<HttpResponseBase>();
            theHttpContext.Stub(x => x.Response).Return(theHttpResponse);

            theHttpContext.Stub(x => x.Items).Return(new Dictionary<string, object>());

            theArguments = new AspNetServiceArguments(theRequestContext);
        }


        [Fact]
        public void registers_an_http_context_base()
        {
            theArguments.Get<HttpContextBase>().ShouldNotBeNull();
        }


        [Fact]
        public void should_register_the_http_context_base()
        {
            theArguments.Get<HttpContextBase>().ShouldBeTheSameAs(theHttpContext);
        }

        [Fact]
        public void should_register_a_current_request_implementation()
        {
            theArguments.Get<IHttpRequest>().ShouldBeOfType<AspNetHttpRequest>();
        }

        [Fact]
        public void should_register_an_HttpWriter()
        {
            theArguments.Get<IHttpResponse>().ShouldBeOfType<AspNetHttpResponse>();
        }
    }
}