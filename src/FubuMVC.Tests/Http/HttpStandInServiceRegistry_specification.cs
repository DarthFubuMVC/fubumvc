using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http
{
    [TestFixture]
    public class HttpStandInServiceRegistry_specification
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            BehaviorGraph.BuildEmptyGraph().Services.DefaultServiceFor<TService>().Type.ShouldEqual(
                typeof(TImplementation));
        }

        [Test]
        public void standin_current_http_request_is_used_as_the_default()
        {
            registeredTypeIs<IHttpRequest, StandInHttpRequest>();
        }

        [Test]
        public void IOwinResponse_is_nullo()
        {
            registeredTypeIs<IHttpResponse, NulloHttpResponse>();
        }

    }
}