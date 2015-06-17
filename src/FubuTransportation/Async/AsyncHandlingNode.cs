using FubuMVC.Core.Registration.Nodes;

namespace FubuTransportation.Async
{
    public class AsyncHandlingNode : Wrapper
    {
        public AsyncHandlingNode() : base(typeof (AsyncHandlingBehavior))
        {
        }
    }
}