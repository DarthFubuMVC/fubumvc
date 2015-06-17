using FubuCore.Logging;

namespace FubuTransportation.Runtime.Invocation
{
    public interface IHandlerPipeline
    {
        void Invoke(Envelope envelope);
        void Receive(Envelope envelope);
        ILogger Logger { get; }
    }
}