using System;
using System.IO;

namespace LightningQueues.Net
{
    public interface IReceivingProtocol
    {
        IObservable<Message> ReceiveStream(IObservable<Stream> streams, string from);
    }
}
