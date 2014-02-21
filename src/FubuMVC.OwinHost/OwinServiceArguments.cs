using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using FubuCore.Binding;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost
{
    public class OwinServiceArguments : ServiceArguments
    {
        public OwinServiceArguments(RouteData routeData, IDictionary<string, object> environment)
        {
            var httpRequest = new OwinCurrentHttpRequest(environment);

            var httpContextBase = new OwinHttpContext(environment);
            With<HttpContextBase>(httpContextBase);

            With(routeData);

            With<ICurrentHttpRequest>(httpRequest);
            With<IHttpResponse>(new OwinHttpResponse(environment));

            With(new OwinContext(environment));
        }
    }
}