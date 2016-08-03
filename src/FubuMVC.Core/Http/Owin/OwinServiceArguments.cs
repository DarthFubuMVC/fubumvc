using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Http.Owin
{
    public class OwinServiceArguments : TypeArguments
    {
        public OwinServiceArguments(RouteData routeData, IDictionary<string, object> environment)
        {
            var httpRequest = new OwinHttpRequest(environment);

            var httpContextBase = new OwinHttpContext(environment);

            Set(environment);

            Set<HttpContextBase>(httpContextBase);

            Set(routeData);

            Set<IHttpRequest>(httpRequest);

            Set<IHttpResponse>(new OwinHttpResponse(environment));

            Set(new OwinContext(environment));

            var log = environment.Log();
            if (log != null)
            {
                Set(log);
            }
        }
    }
}