using System.Collections.Generic;

namespace FubuTransportation.Runtime.Cascading
{
    public interface IOutgoingSender
    {
        void SendOutgoingMessages(Envelope original, IEnumerable<object> cascadingMessages);
        void SendFailureAcknowledgement(Envelope original, string message);

        void Send(Envelope envelope);
    }
}