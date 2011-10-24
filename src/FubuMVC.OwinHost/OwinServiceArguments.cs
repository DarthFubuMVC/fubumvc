using System.Web.Routing;
using FubuCore.Binding;
using FubuMVC.Core.Http;
using Gate.Helpers;

namespace FubuMVC.OwinHost
{
    public class OwinServiceArguments : ServiceArguments
    {
        public OwinServiceArguments(RouteData routeData, Request request, Response response)
        {
            With<AggregateDictionary>(new OwinAggregateDictionary(routeData, request));

            With<ICurrentHttpRequest>(new OwinCurrentHttpRequest(request));
            With<IStreamingData>(new OwinStreamingData(request, response));
            With<IHttpWriter>(new OwinHttpWriter(response));
        }
    }
}