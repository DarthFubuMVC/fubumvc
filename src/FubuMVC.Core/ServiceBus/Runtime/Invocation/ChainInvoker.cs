using System;
using System.Runtime.Serialization;
using FubuCore;
using FubuCore.Dates;
using FubuCore.Logging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public class ChainInvoker : IChainInvoker
    {
        private readonly IServiceFactory _factory;
        private readonly BehaviorGraph _graph;
        private readonly ILogger _logger;
        private readonly IOutgoingSender _sender;
        private readonly Func<IHandlerPipeline> _pipeline;

        public ChainInvoker(IServiceFactory factory, BehaviorGraph graph, ILogger logger, IOutgoingSender sender, Func<IHandlerPipeline> pipeline)
        {
            _factory = factory;
            _graph = graph;
            _logger = logger;
            _sender = sender;
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
            _pipeline().InvokeNow(envelope);
        }

        public virtual HandlerChain FindChain(Envelope envelope)
        {
            var messageType = envelope.Message.GetType();
            return _graph.ChainFor(messageType).As<HandlerChain>();
        }


        public IInvocationContext ExecuteChain(Envelope envelope, HandlerChain chain)
        {
            if (envelope.Log != null)
            {
                envelope.Log.RootChain = chain;
                envelope.Log.StartSubject(chain);
            }

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

                    if (envelope.Log != null)
                    {
                        envelope.Log.FinishSubject();
                    }
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

    [Serializable]
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