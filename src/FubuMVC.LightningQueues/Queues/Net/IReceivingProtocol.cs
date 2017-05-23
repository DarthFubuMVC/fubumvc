using System;
using System.IO;

namespace FubuMVC.LightningQueues.Queues.Net
{
    public interface IReceivingProtocol
    {
        IObservable<Message> ReceiveStream(IObservable<Stream> streams, string from);
    }
}
