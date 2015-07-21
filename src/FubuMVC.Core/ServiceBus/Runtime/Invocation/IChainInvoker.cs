using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public interface IChainInvoker
    {
        void Invoke(Envelope envelope);
        void InvokeNow<T>(T message);
        IInvocationContext ExecuteChain(Envelope envelope, HandlerChain chain);
        HandlerChain FindChain(Envelope envelope);
    }
}