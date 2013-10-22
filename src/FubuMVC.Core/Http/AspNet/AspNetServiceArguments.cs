using System.Web.Routing;
using FubuCore.Binding;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetServiceArguments : ServiceArguments
    {
        public AspNetServiceArguments(RequestContext requestContext)
        {
            var currentRequest = new AspNetCurrentHttpRequest(requestContext.HttpContext.Request);

            With<IRequestData>(new AspNetRequestData(requestContext, currentRequest));
            With(requestContext.HttpContext);
            //With(requestContext.HttpContext.Session);

            
            With<ICurrentHttpRequest>(currentRequest);

            With<IStreamingData>(new AspNetStreamingData(requestContext.HttpContext));

            With<IHttpWriter>(new AspNetHttpWriter(requestContext.HttpContext.Response));

            With<IClientConnectivity>(new AspNetClientConnectivity(requestContext.HttpContext.Response));

            With<IResponse>(new AspNetResponse(requestContext.HttpContext.Response));
        }

    }

}