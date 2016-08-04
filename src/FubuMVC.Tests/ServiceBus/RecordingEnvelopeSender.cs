using System.Collections.Generic;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Tests.ServiceBus
{
    public class RecordingEnvelopeSender : IEnvelopeSender, IOutgoingSender
    {
        public readonly IList<Envelope> Sent = new List<Envelope>(); 
        public readonly IList<object> Outgoing = new List<object>(); 

        

        public string Send(Envelope envelope)
        {
            Sent.Add(envelope);

            return envelope.CorrelationId;
        }

        public void SendOutgoingMessages(Envelope original, IEnumerable<object> cascadingMessages)
        {
            Outgoing.AddRange(cascadingMessages);
        }

        public void SendFailureAcknowledgement(Envelope original, string message)
        {
            FailureAcknowledgementMessage = message;
        }

        void IOutgoingSender.Send(Envelope envelope)
        {
            Sent.Add(envelope);
        }

        public string Send(Envelope envelope, IMessageCallback callback)
        {
            Sent.Add(envelope);
            return envelope.CorrelationId;
        }

        public string FailureAcknowledgementMessage { get; set; }
    }
}