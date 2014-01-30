using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.OwinHost.Middleware
{
    public class MiddlewareChain : Chain<MiddlewareNode, MiddlewareChain>
    {
    }
}