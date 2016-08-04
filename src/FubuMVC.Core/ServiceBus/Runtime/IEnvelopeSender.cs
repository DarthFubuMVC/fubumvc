using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.Runtime
{
    public interface IEnvelopeSender
    {
        string Send(Envelope envelope);
        string Send(Envelope envelope, IMessageCallback callback);
    }
}