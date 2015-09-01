using FubuCore.Logging;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public interface IHandlerPipeline
    {
        void Invoke(Envelope envelope);
        void Receive(Envelope envelope);
    }
}