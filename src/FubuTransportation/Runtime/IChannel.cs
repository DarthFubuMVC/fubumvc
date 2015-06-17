using System;
using FubuTransportation.Runtime.Headers;

namespace FubuTransportation.Runtime
{
    public interface IChannel : IDisposable
    {
        Uri Address { get; }
        ReceivingState Receive(IReceiver receiver);
        void Send(byte[] data, IHeaders headers);
    }

    public enum ReceivingState
    {
        CanContinueReceiving,
        StopReceiving
    }
}