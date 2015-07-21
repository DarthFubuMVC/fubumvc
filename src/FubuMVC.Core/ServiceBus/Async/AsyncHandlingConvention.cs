using System.Linq;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Registration.Nodes;

namespace FubuMVC.Core.ServiceBus.Async
{
    [System.ComponentModel.Description("Adds an AsyncHandlingNode for asynchronous handler chains")]
    public class AsyncHandlingConvention : HandlerChainPolicy    
    {
        public override bool Matches(HandlerChain chain)
        {
            return chain.IsAsync;
        }

        public override void Configure(HandlerChain handlerChain)
        {
            var firstCall = handlerChain.OfType<HandlerCall>().First();
            firstCall.AddBefore(new AsyncHandlingNode());
        }
    }
}