using System.Web;
using System.Web.Routing;
using FubuCore.Binding;

namespace FubuMVC.Core.Runtime.Handlers
{
    public interface IHttpHandlerSource
    {
        IHttpHandler Build(IBehaviorInvoker invoker, ServiceArguments arguments, RouteValueDictionary routeValues);
    }
}