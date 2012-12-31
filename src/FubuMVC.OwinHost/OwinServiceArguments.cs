using System.Collections.Generic;
using System.Web.Routing;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Cookies;

namespace FubuMVC.OwinHost
{
    public class OwinServiceArguments : ServiceArguments
    {
        public OwinServiceArguments(RouteData routeData, IDictionary<string, object> environment)
        {
            var headers = environment.Get<IDictionary<string, string[]>>(OwinConstants.RequestHeadersKey);


            With<IRequestData>(new OwinRequestData(routeData, environment, headers));

            With<ICurrentHttpRequest>(new OwinCurrentHttpRequest(environment));
            With<IStreamingData>(new OwinStreamingData(environment));
            With<IHttpWriter>(new OwinHttpWriter(environment));

            With<IResponse>(new OwinResponse(environment));
        }
    }
}