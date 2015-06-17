using System.Linq;
using FubuMVC.Core;
using FubuTransportation.Configuration;
using FubuTransportation.Registration.Nodes;

namespace FubuTransportation.Async
{
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