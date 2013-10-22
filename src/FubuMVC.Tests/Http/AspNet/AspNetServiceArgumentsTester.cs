using System.Web;
using System.Web.Routing;
using FubuCore.Binding;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.AspNet;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace FubuMVC.Tests.Http.AspNet
{
    [TestFixture]
    public class AspNetServiceArgumentsTester
    {
        private HttpContextBase theHttpContext;
        private RequestContext theRequestContext;
        private AspNetServiceArguments theArguments;
        private HttpRequestBase theHttpRequest;
        private HttpResponseBase theHttpResponse;

        [SetUp]
        public void SetUp()
        {
            theHttpContext = MockRepository.GenerateMock<HttpContextBase>();
            theRequestContext = new RequestContext(theHttpContext, new RouteData());

            theHttpRequest = MockRepository.GenerateMock<HttpRequestBase>();
            theHttpContext.Stub(x => x.Request).Return(theHttpRequest);

            theHttpResponse = MockRepository.GenerateMock<HttpResponseBase>();
            theHttpContext.Stub(x => x.Response).Return(theHttpResponse);

            theArguments = new AspNetServiceArguments(theRequestContext);
        }

        [Test]
        public void should_have_the_client_connectivity()
        {
            theArguments.Get<IClientConnectivity>().ShouldBeOfType<AspNetClientConnectivity>();
        }

        [Test]
        public void registers_an_http_context_base()
        {
            theArguments.Get<HttpContextBase>().ShouldNotBeNull();
        }


        [Test]
        public void should_add_the_request_data()
        {
            theArguments.Get<IRequestData>().ShouldBeOfType<AspNetRequestData>();
        }

        [Test]
        public void should_register_the_http_context_base()
        {
            theArguments.Get<HttpContextBase>().ShouldBeTheSameAs(theHttpContext);
        }

        [Test]
        public void should_register_a_current_request_implementation()
        {
            theArguments.Get<ICurrentHttpRequest>().ShouldBeOfType<AspNetCurrentHttpRequest>();
        }

        [Test]
        public void should_register_a_streaming_data_implementation()
        {
            theArguments.Get<IStreamingData>().ShouldBeOfType<AspNetStreamingData>();
        }

        [Test]
        public void should_register_an_HttpWriter()
        {
            theArguments.Get<IHttpWriter>().ShouldBeOfType<AspNetHttpWriter>();
        }
    }
}