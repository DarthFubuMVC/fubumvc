using System.Web;
using System.Web.Routing;
using FubuCore.Binding;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetServiceArguments : ServiceArguments
    {
        public AspNetServiceArguments(RequestContext requestContext)
        {
            var currentRequest = new AspNetHttpRequest(requestContext.HttpContext.Request, requestContext.HttpContext.Response);

            With(requestContext.RouteData);
            With(requestContext.HttpContext);
            //With(requestContext.HttpContext.Session);

            
            With<IHttpRequest>(currentRequest);

            With<IHttpResponse>(new AspNetHttpResponse(requestContext.HttpContext.Response));
        }

    }

}