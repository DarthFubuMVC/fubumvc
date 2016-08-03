using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core.Diagnostics.Instrumentation;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetServiceArguments : TypeArguments
    {
        public const string CHAIN_EXECUTION_LOG = "ChainExecutionLog";

        public AspNetServiceArguments(RequestContext requestContext)
        {
            var currentRequest = new AspNetHttpRequest(requestContext.HttpContext.Request, requestContext.HttpContext.Response);

            if (requestContext.HttpContext.Items.Contains(CHAIN_EXECUTION_LOG))
            {
                var log = requestContext.HttpContext.Items[CHAIN_EXECUTION_LOG].As<IChainExecutionLog>();
                Set(log);
            }

            Set(requestContext.RouteData);
            Set(requestContext.HttpContext);
            //With(requestContext.HttpContext.Session);

            
            Set<IHttpRequest>(currentRequest);

            Set<IHttpResponse>(new AspNetHttpResponse(requestContext.HttpContext.Response));
        }

    }

}