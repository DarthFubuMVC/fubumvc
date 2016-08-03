using System.Web;
using System.Web.Routing;
using FubuCore.Binding;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Runtime.Handlers
{
    public class SessionlessSynchronousHttpHandlerSource : IHttpHandlerSource
    {
        public IHttpHandler Build(IBehaviorInvoker invoker, TypeArguments arguments, RouteValueDictionary routeValues)
        {
            return new SessionLessFubuHttpHandler(invoker, arguments, routeValues);
        }
    }
}