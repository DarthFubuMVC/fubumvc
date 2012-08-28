using System.Web;
using System.Web.Routing;
using FubuCore.Binding;

namespace FubuMVC.Core.Runtime.Handlers
{
    public class SynchronousHttpHandlerSource : IHttpHandlerSource
    {
        public IHttpHandler Build(IBehaviorInvoker invoker, ServiceArguments arguments, RouteValueDictionary routeValues)
        {
            return new SynchronousFubuHttpHandler(invoker, arguments, routeValues);
        }
    }
}