using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using FubuCore.Binding;

namespace FubuMVC.Core.Http.Owin
{
    public class OwinServiceArguments : ServiceArguments
    {
        public OwinServiceArguments(RouteData routeData, IDictionary<string, object> environment)
        {
            var httpRequest = new OwinHttpRequest(environment);

            var httpContextBase = new OwinHttpContext(environment);

            With(environment);

            With<HttpContextBase>(httpContextBase);

            With(routeData);

            With<IHttpRequest>(httpRequest);

            With<IHttpResponse>(new OwinHttpResponse(environment));

            With(new OwinContext(environment));

            var log = environment.Log();
            if (log != null)
            {
                With(log);
            }
        }
    }
}