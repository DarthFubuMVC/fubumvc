using System.Web;
using System.Web.Routing;
using FubuMVC.Core.Http.AspNet;

namespace FubuMVC.Core.Runtime.Handlers
{
    public class FubuRouteHandler : IFubuRouteHandler
    {
        private readonly IBehaviorInvoker _invoker;
        private readonly IHttpHandlerSource _handlerSource;

        public FubuRouteHandler(IBehaviorInvoker invoker, IHttpHandlerSource handlerSource)
        {
            _invoker = invoker;
            _handlerSource = handlerSource;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var arguments = new AspNetServiceArguments(requestContext);
            return _handlerSource.Build(_invoker, arguments, requestContext.RouteData.Values);
        }

        public IBehaviorInvoker Invoker
        {
            get { return _invoker; }
        }
    }
}