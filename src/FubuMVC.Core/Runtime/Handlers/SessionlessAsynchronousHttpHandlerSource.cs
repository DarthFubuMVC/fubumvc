using System.Web;
using System.Web.Routing;
using FubuCore.Binding;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Runtime.Handlers
{
    public class SessionlessAsynchronousHttpHandlerSource : IHttpHandlerSource
    {
        public IHttpHandler Build(IBehaviorInvoker invoker, TypeArguments arguments, RouteValueDictionary routeValues)
        {
            return new SessionlessAsynchronousHttpHandler(invoker, arguments, routeValues);
        }
    }
}