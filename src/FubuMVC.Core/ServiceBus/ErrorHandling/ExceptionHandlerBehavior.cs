using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuCore.Logging;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.ErrorHandling
{
    public class ExceptionHandlerBehavior : IActionBehavior
    {
        private readonly IActionBehavior _behavior;
        private readonly Envelope _envelope;
        private readonly IInvocationContext _context;
        private readonly ILogger _logger;
        private readonly IFubuRequest _request;

        public ExceptionHandlerBehavior(IActionBehavior behavior, HandlerChain chain, Envelope envelope, IInvocationContext context, ILogger logger, IFubuRequest request)
        {
            _behavior = behavior;
            Chain = chain;
            _envelope = envelope;
            _context = context;
            _logger = logger;
            _request = request;
        }

        public async Task Invoke()
        {
            try
            {
                await _behavior.Invoke().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var message = _request.Get(Chain.InputType());

                if (message == null)
                {
                    _logger.Error(_envelope.CorrelationId, ex);
                }
                else
                {
                    _logger.Error(_envelope.CorrelationId, "Error trying to process " + message, ex);
                }

                _context.Continuation = DetermineContinuation(ex);
            }
        }

        public IContinuation DetermineContinuation(Exception ex)
        {
            if (_envelope.Attempts >= Chain.MaximumAttempts)
            {
                return new MoveToErrorQueue(ex);
            }

            return tryToDetermineContinuation(ex)
                   ?? new MoveToErrorQueue(ex);
        }

        private IContinuation tryToDetermineContinuation(Exception ex)
        {
            var aggregate = ex as AggregateException;
            if (aggregate != null)
            {
                var exceptions = aggregate.Flatten().InnerExceptions;
                return exceptions.FirstValue(tryToDetermineContinuation);
            }
            
            return Chain.ErrorHandlers.FirstValue(x => x.DetermineContinuation(_envelope, ex));
        }

        public HandlerChain Chain { get; }

        public Task InvokePartial()
        {
            return _behavior.InvokePartial();
        }
    }
}