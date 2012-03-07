using System.Web.Routing;
using FubuCore.Binding;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost
{
    public class OwinServiceArguments : ServiceArguments
    {
        public OwinServiceArguments(RouteData routeData, OwinRequestBody body, Response response)
        {
            With<IRequestData>(new OwinRequestData(routeData, body));

            With<ICurrentHttpRequest>(new OwinCurrentHttpRequest(body));
            With<IStreamingData>(new OwinStreamingData(body));
            With<IHttpWriter>(new OwinHttpWriter(response));
        }
    }
}