using System.Web;
using System.Web.Routing;
using FubuCore.Binding;

namespace FubuMVC.Core.Runtime.Handlers
{
    public class SessionlessAsynchronousHttpHandlerSource : IHttpHandlerSource
    {
        public IHttpHandler Build(IBehaviorInvoker invoker, ServiceArguments arguments, RouteValueDictionary routeValues)
        {
            return new SessionlessAsynchronousHttpHandler(invoker, arguments, routeValues);
        }
    }
}