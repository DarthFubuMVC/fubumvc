using System.Web;
using System.Web.Routing;
using FubuCore.Binding;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Runtime.Handlers
{
    public class SynchronousHttpHandlerSource : IHttpHandlerSource
    {
        public IHttpHandler Build(IBehaviorInvoker invoker, TypeArguments arguments, RouteValueDictionary routeValues)
        {
            return new SynchronousFubuHttpHandler(invoker, arguments, routeValues);
        }
    }
}