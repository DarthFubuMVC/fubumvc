using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.ServiceBus.Async
{
    public class AsyncHandlingNode : Wrapper
    {
        public AsyncHandlingNode() : base(typeof (AsyncHandlingBehavior))
        {
        }
    }
}