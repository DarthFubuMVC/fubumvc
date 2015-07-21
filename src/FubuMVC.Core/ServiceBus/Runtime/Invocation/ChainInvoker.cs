using System;
using System.Runtime.Serialization;
using FubuCore;
using FubuCore.Dates;
using FubuCore.Logging;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public class ChainInvoker : IChainInvoker
    {
        private readonly IServiceFactory _factory;
        private readonly HandlerGraph _graph;
        private readonly ILogger _logger;
        private readonly IOutgoingSender _sender;
        private readonly ISystemTime _systemTime;
        private readonly Func<IHandlerPipeline> _pipeline;

        public ChainInvoker(IServiceFactory factory,
            HandlerGraph graph,
            ILogger logger,
            IOutgoingSender sender,
            ISystemTime systemTime,
            Func<IHandlerPipeline> pipeline)
        {
            _factory = factory;
            _graph = graph;
            _logger = logger;
            _sender = sender;
            _systemTime = systemTime;
            _pipeline = pipeline;
        }

        public void Invoke(Envelope envelope)
        {
            var chain = FindChain(envelope);

            ExecuteChain(envelope, chain);
        }

        public void InvokeNow<T>(T message)
        {
            var envelope = new Envelope { Message = message };
            envelope.Callback = new InlineMessageCallback(message, _sender);
            _pipeline().Invoke(envelope);
        }

        public virtual HandlerChain FindChain(Envelope envelope)
        {
            var messageType = envelope.Message.GetType();
            return _graph.ChainFor(messageType);
        }


        public IInvocationContext ExecuteChain(Envelope envelope, HandlerChain chain)
        {
            using (new ChainExecutionWatcher(_logger, chain, envelope))
            {
                var context = new InvocationContext(envelope, chain);
                var behavior = _factory.BuildBehavior(context, chain.UniqueId);

                try
                {
                    behavior.Invoke();
                }
                finally
                {
                    (behavior as IDisposable).CallIfNotNull(x => x.SafeDispose());
                }

                return context;
            }
        }
    }

    public class InlineMessageCallback : IMessageCallback
    {
        private readonly object _message;
        private readonly IOutgoingSender _sender;

        public InlineMessageCallback(object message, IOutgoingSender sender)
        {
            _message = message;
            _sender = sender;
        }

        public void MarkSuccessful()
        {
        }

        public void MarkFailed(Exception ex)
        {
            throw new InlineMessageException("Failed while invoking an inline message", ex);
        }

        public void MoveToDelayedUntil(DateTime time)
        {
            _sender.Send(new Envelope
            {
                Message = _message,
                ExecutionTime = time
            });
        }

        public void MoveToErrors(ErrorReport report)
        {
            // TODO -- need a general way to log errors against an ITransport
        }

        public void Requeue()
        {
            _sender.Send(new Envelope
            {
                Message = _message
            });
        }
    }

    public class InlineMessageException : Exception
    {
        public InlineMessageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InlineMessageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}