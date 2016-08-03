using System.Web;
using System.Web.Routing;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Runtime.Handlers
{
    public interface IHttpHandlerSource
    {
        IHttpHandler Build(IBehaviorInvoker invoker, TypeArguments arguments, RouteValueDictionary routeValues);
    }
}