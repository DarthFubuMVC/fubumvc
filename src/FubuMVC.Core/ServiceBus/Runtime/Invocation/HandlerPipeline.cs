using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Logging;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public class HandlerPipeline : IHandlerPipeline
    {
        private readonly IEnvelopeSerializer _serializer;
        private readonly EnvelopeContext _context;
        private readonly IList<IEnvelopeHandler> _handlers = new List<IEnvelopeHandler>();

        public HandlerPipeline(IEnvelopeSerializer serializer, EnvelopeContext context,
            IEnumerable<IEnvelopeHandler> handlers)
        {
            _serializer = serializer;
            _context = context;
            _handlers.AddRange(handlers);

            // needs to be available to continuations
            _context.Pipeline = this;
        }

        public IList<IEnvelopeHandler> Handlers
        {
            get { return _handlers; }
        }

        // virtual for testing as usual
        public virtual IContinuation FindContinuation(Envelope envelope)
        {
            foreach (var handler in _handlers)
            {
                var continuation = handler.Handle(envelope);
                if (continuation != null)
                {
                    _context.DebugMessage(() => new EnvelopeContinuationChosen
                    {
                        ContinuationType = continuation.GetType(),
                        HandlerType = handler.GetType(),
                        Envelope = envelope.ToToken()
                    });

                    return continuation;
                }
            }

            // TODO - add rules for what to do when we have no handler
            return new MoveToErrorQueue(new NoHandlerException(envelope.Message.GetType()));
        }

        public virtual void Invoke(Envelope envelope)
        {
            envelope.Attempts++; // needs to be done here.
            IContinuation continuation = null;

            try
            {
                continuation = FindContinuation(envelope);
                continuation.Execute(envelope, _context);
            }
            catch (EnvelopeDeserializationException ex)
            {
                new DeserializationFailureContinuation(ex).Execute(envelope, _context);
            }
            catch (Exception e)
            {
                envelope.Callback.MarkFailed(e); // TODO -- watch this one.
                _context.Error(envelope.CorrelationId,
                    "Failed while invoking message {0} with continuation {1}".ToFormat(envelope.Message ?? envelope,
                        (object)continuation ?? "could not find continuation"),
                    e);
            }
        }

        public void Receive(Envelope envelope)
        {
            envelope.UseSerializer(_serializer);
            _context.InfoMessage(() => new EnvelopeReceived { Envelope = envelope.ToToken() });

            Invoke(envelope);
        }
    }
}