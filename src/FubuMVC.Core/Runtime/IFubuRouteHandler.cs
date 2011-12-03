using System.Web.Routing;

namespace FubuMVC.Core.Runtime
{
    public interface IFubuRouteHandler : IRouteHandler
    {
        IBehaviorInvoker Invoker { get; }
    }
}