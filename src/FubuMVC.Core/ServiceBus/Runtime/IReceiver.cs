using FubuMVC.Core.ServiceBus.Runtime.Headers;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.Runtime
{
    public interface IReceiver
    {
        void Receive(byte[] data, IHeaders headers, IMessageCallback callback);
    }
}