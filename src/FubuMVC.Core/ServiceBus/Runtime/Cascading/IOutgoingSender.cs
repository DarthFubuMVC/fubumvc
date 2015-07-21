using System.Collections.Generic;

namespace FubuMVC.Core.ServiceBus.Runtime.Cascading
{
    public interface IOutgoingSender
    {
        void SendOutgoingMessages(Envelope original, IEnumerable<object> cascadingMessages);
        void SendFailureAcknowledgement(Envelope original, string message);

        void Send(Envelope envelope);
    }
}