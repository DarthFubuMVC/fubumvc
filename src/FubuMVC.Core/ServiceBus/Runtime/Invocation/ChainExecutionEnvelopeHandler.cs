using System;
using FubuMVC.Core.ServiceBus.Async;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public class ChainExecutionEnvelopeHandler : IEnvelopeHandler
    {
        private readonly IChainInvoker _invoker;

        public ChainExecutionEnvelopeHandler(IChainInvoker invoker)
        {
            _invoker = invoker;
        }

        public IContinuation Handle(Envelope envelope)
        {
            try
            {
                var chain = _invoker.FindChain(envelope);
                if (chain == null)
                {
                    return null;
                }

                return chain.IsAsync
                    ? new AsyncChainExecutionContinuation(() => ExecuteChain(envelope, chain))
                    : ExecuteChain(envelope, chain);

            }
            catch (EnvelopeDeserializationException ex)
            {
                return new DeserializationFailureContinuation(ex);
            }
        }

        public IContinuation ExecuteChain(Envelope envelope, HandlerChain chain)
        {
            try
            {
                var context = _invoker.ExecuteChain(envelope, chain);
                return context.Continuation ?? new ChainSuccessContinuation(context);
            }
            catch (EnvelopeDeserializationException ex)
            {
                return new DeserializationFailureContinuation(ex);
            }
            catch (Exception ex)
            {
                // TODO -- might be nice to capture the Chain
                return new ChainFailureContinuation(ex);
            }
        }
    }
}