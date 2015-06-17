using FubuTransportation.Runtime.Headers;
using FubuTransportation.Runtime.Invocation;

namespace FubuTransportation.Runtime
{
    public interface IReceiver
    {
        void Receive(byte[] data, IHeaders headers, IMessageCallback callback);
    }
}