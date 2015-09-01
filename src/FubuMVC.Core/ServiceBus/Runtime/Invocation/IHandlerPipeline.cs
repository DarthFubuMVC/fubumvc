using FubuCore.Logging;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public interface IHandlerPipeline
    {
        void Invoke(Envelope envelope, IEnvelopeContext context);
        void Receive(Envelope envelope);
        void InvokeNow(Envelope envelope);
    }
}