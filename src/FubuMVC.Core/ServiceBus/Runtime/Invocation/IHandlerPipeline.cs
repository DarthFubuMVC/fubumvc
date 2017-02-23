using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public interface IHandlerPipeline
    {
        void Invoke(Envelope envelope, IEnvelopeContext context);
        void Receive(Envelope envelope, ChannelNode node);
        void InvokeNow(Envelope envelope);
    }
}
