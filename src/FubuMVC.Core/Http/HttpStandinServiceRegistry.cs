using System.Web.Routing;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Http
{
    public class HttpStandInServiceRegistry : ServiceRegistry
    {
        public HttpStandInServiceRegistry()
        {
            SetServiceIfNone<IHttpRequest>(new OwinHttpRequest());

            SetServiceIfNone<IHttpResponse>(new OwinHttpResponse());

            SetServiceIfNone<ICurrentChain>(new CurrentChain(null, null));

            SetServiceIfNone(new RouteData());
        }
    }
}