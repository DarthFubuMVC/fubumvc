using System;
using System.Collections.Generic;
using System.Diagnostics;
using FubuCore;
using FubuCore.Logging;

namespace FubuMVC.Core.ServiceBus.Runtime.Cascading
{
    public class OutgoingSender : IOutgoingSender
    {
        private readonly IEnvelopeSender _sender;
        private readonly ILogger _logger;

        public OutgoingSender(IEnvelopeSender sender, ILogger logger)
        {
            _sender = sender;
            _logger = logger;
        }

        public void SendOutgoingMessages(Envelope original, IEnumerable<object> cascadingMessages)
        {
            if (original.AckRequested)
            {
                sendAcknowledgement(original);
            }

            cascadingMessages.Each(o => SendOutgoingMessage(original, o));
        }

        public void SendFailureAcknowledgement(Envelope original, string message)
        {
            if (original.AckRequested || original.ReplyRequested.IsNotEmpty())
            {
                var envelope = new Envelope
                {
                    ParentId = original.CorrelationId,
                    Destination = original.ReplyUri,
                    ResponseId = original.CorrelationId,
                    Message = new FailureAcknowledgement()
                    {
                        CorrelationId = original.CorrelationId, 
                        Message = message
                    },
                    Callback = original.Callback
                };

                Send(envelope);
            }
        }

        public void Send(Envelope envelope)
        {
            try
            {
                if (envelope.Callback.SupportsSend)
                {
                    Debug.WriteLine("Using the new IMessageCallback.Send() for cascading messages");
                    _sender.Send(envelope, envelope.Callback);
                }
                else
                {
                    _sender.Send(envelope);
                }


            }
            catch (Exception e)
            {
                // TODO -- we really, really have to do something here
                _logger.Error(envelope.OriginalId, "Failure while trying to send a cascading message", e);
            }
        }

        private void sendAcknowledgement(Envelope original)
        {
            var envelope = new Envelope
            {
                ParentId = original.CorrelationId,
                Destination = original.ReplyUri,
                ResponseId = original.CorrelationId,
                Message = new Acknowledgement {CorrelationId = original.CorrelationId}
            };

            Send(envelope);
        }

        public void SendOutgoingMessage(Envelope original, object o)
        {
            var cascadingEnvelope = o is ISendMyself
                ? o.As<ISendMyself>().CreateEnvelope(original)
                : original.ForResponse(o);

            cascadingEnvelope.Callback = original.Callback;

            Send(cascadingEnvelope);
        }
    }
}