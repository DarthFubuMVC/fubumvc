using System.Collections.Generic;
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

            With<IRequestData>(new OwinRequestData(routeData, environment, httpRequest));


            With<ICurrentHttpRequest>(httpRequest);
            With<IStreamingData>(new OwinStreamingData(environment));
            With<IHttpWriter>(new OwinHttpWriter(environment));

            With<IResponse>(new OwinResponse(environment));
        }
    }
}