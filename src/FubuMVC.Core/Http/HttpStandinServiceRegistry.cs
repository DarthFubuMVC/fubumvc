using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Http
{
    public class HttpStandInServiceRegistry : ServiceRegistry
    {
        public HttpStandInServiceRegistry()
        {
            // All of these services are replaced per request in nested
            // containers, but this is necessary for testing
            For<IHttpRequest>().Use<OwinHttpRequest>();
            For<IHttpResponse>().Use<OwinHttpResponse>();
            For<ICurrentChain>().Use(new CurrentChain(null, null));
            For<RouteData>().Use(new RouteData());
            For<IDictionary<string, object>>().Use(new Dictionary<string, object>());

            // This is important, really needs to be the nullo to avoid
            // hanging on to things we shouldn't
            For<IChainExecutionLog>().Use<NulloChainExecutionLog>();
        }
    }
}