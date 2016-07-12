using System;
using System.Threading.Tasks;
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

        public async Task<IContinuation> Handle(Envelope envelope)
        {
            try
            {
                var chain = _invoker.FindChain(envelope);
                if (chain == null)
                {
                    return null;
                }

                return await ExecuteChain(envelope, chain).ConfigureAwait(false);

            }
            catch (EnvelopeDeserializationException ex)
            {
                return new DeserializationFailureContinuation(ex);
            }
        }

        public async Task<IContinuation> ExecuteChain(Envelope envelope, HandlerChain chain)
        {
            try
            {
                var context = await _invoker.ExecuteChain(envelope, chain).ConfigureAwait(false);
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