using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.OwinHost
{
    public class MiddlewareChain : Chain<MiddlewareNode, MiddlewareChain>
    {
    }
}