using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly IEnvelopeLifecycle _lifecycle;
        private readonly IList<IEnvelopeHandler> _handlers = new List<IEnvelopeHandler>();

        public HandlerPipeline(IEnvelopeSerializer serializer, IEnvelopeLifecycle lifecycle, IEnumerable<IEnvelopeHandler> handlers)
        {
            _serializer = serializer;
            _lifecycle = lifecycle;
            _handlers.AddRange(handlers);
        }

        public IList<IEnvelopeHandler> Handlers => _handlers;

        // virtual for testing as usual
        public virtual async Task<IContinuation> FindContinuation(Envelope envelope, IEnvelopeContext context)
        {
            foreach (var handler in _handlers)
            {
                var continuation = await handler.Handle(envelope).ConfigureAwait(false);
                if (continuation != null)
                {
                    context.DebugMessage(() => new EnvelopeContinuationChosen
                    {   
                        ContinuationType = continuation.GetType(),
                        HandlerType = handler.GetType(),
                        Envelope = envelope.ToToken()
                    });

                    return continuation;
                }
            }

            // TODO - add rules for what to do when we have no handler
            context.DebugMessage(() => new EnvelopeContinuationChosen
            {
                ContinuationType = typeof(MoveToErrorQueue),
                HandlerType = typeof(HandlerPipeline),
                Envelope = envelope.ToToken()
            });

            return new MoveToErrorQueue(new NoHandlerException(envelope.Message.GetType()));
        }

        public virtual async Task Invoke(Envelope envelope, IEnvelopeContext context)
        {
            envelope.Attempts++; // needs to be done here.
            IContinuation continuation = null;

            try
            {
                continuation = await FindContinuation(envelope, context).ConfigureAwait(false);
                continuation.Execute(envelope, context);
            }
            catch (EnvelopeDeserializationException ex)
            {
                new DeserializationFailureContinuation(ex).Execute(envelope, context);
            }
            catch (AggregateException e)
            {
                e.Flatten().InnerExceptions.Each(ex =>
                {
                    envelope.Callback.MarkFailed(e);
                    var message = "Failed while invoking message {0} with continuation {1}".ToFormat(envelope.Message ?? envelope,
                        (object)continuation ?? "could not find continuation");

                    context.Error(envelope.CorrelationId, message, e);
                });
            }
            catch (Exception e)
            {
                envelope.Callback.MarkFailed(e); // TODO -- watch this one.
                var message = "Failed while invoking message {0} with continuation {1}".ToFormat(envelope.Message ?? envelope,
                    (object)continuation ?? "could not find continuation");
                context.Error(envelope.CorrelationId, message, e);
            }
        }

        public async Task Receive(Envelope envelope)
        {
            envelope.UseSerializer(_serializer);
            using (var context = _lifecycle.StartNew(this, envelope))
            {
                context.InfoMessage(() => new EnvelopeReceived { Envelope = envelope.ToToken() });

                // TODO -- this needs to change in all likelihood
                await Invoke(envelope, context).ConfigureAwait(false);
            }


        }

        public async Task InvokeNow(Envelope envelope)
        {
            using (var context = _lifecycle.StartNew(this, envelope))
            {
                await Invoke(envelope, context).ConfigureAwait(false);
            }
        }
    }
}