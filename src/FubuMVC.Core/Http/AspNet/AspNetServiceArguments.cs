using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Diagnostics.Instrumentation;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetServiceArguments : ServiceArguments
    {
        public const string CHAIN_EXECUTION_LOG = "ChainExecutionLog";

        public AspNetServiceArguments(RequestContext requestContext)
        {
            var currentRequest = new AspNetHttpRequest(requestContext.HttpContext.Request, requestContext.HttpContext.Response);

            if (requestContext.HttpContext.Items.Contains(CHAIN_EXECUTION_LOG))
            {
                var log = requestContext.HttpContext.Items[CHAIN_EXECUTION_LOG].As<IChainExecutionLog>();
                With(log);
            }

            With(requestContext.RouteData);
            With(requestContext.HttpContext);
            //With(requestContext.HttpContext.Session);

            
            With<IHttpRequest>(currentRequest);

            With<IHttpResponse>(new AspNetHttpResponse(requestContext.HttpContext.Response));
        }

    }

}