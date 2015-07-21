using System;
using FubuMVC.Core.ServiceBus.Runtime.Headers;

namespace FubuMVC.Core.ServiceBus.Runtime
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