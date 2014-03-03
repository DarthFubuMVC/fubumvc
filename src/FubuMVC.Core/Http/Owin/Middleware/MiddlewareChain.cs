using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Http.Owin.Middleware
{
    public class MiddlewareChain : Chain<MiddlewareNode, MiddlewareChain>
    {
    }
}