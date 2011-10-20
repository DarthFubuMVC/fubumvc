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

        [SetUp]
        public void SetUp()
        {
            theHttpContext = MockRepository.GenerateMock<HttpContextBase>();
            theRequestContext = new RequestContext(theHttpContext, new RouteData());

            theArguments = new AspNetServiceArguments(theRequestContext);
        }

        [Test]
        public void should_have_an_aggregate_dictionary()
        {
            theArguments.Get<AggregateDictionary>().ShouldBeOfType<AspNetAggregateDictionary>();
        }

        [Test]
        public void should_register_the_http_context_base()
        {
            theArguments.Get<HttpContextBase>().ShouldBeTheSameAs(theHttpContext);
        }

        [Test]
        public void should_register_a_current_request_implementation()
        {
            theArguments.Get<ICurrentRequest>().ShouldBeOfType<AspNetCurrentRequest>();
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