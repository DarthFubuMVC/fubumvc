using System;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Logging;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    // Tested only through integration tests
    public class MessageExecutor : IMessageExecutor
    {
        private readonly IPartialFactory _factory;
        private readonly IFubuRequest _request;
        private readonly IChainResolver _resolver;
        private readonly ILogger _logger;
        private readonly Envelope _envelope;

        public MessageExecutor(IPartialFactory factory, IFubuRequest request, IChainResolver resolver, ILogger logger, Envelope envelope)
        {
            _factory = factory;
            _request = request;
            _resolver = resolver;
            _logger = logger;
            _envelope = envelope;
        }

        public bool TryExecute(object message, Action<object> onNoConsumer)
        {
            var inputType = message.GetType();

            var chain = _resolver.FindUniqueByType(inputType);

            if (chain == null)
            {
                if (onNoConsumer != null) onNoConsumer(message);
                return false;
            }

            Execute(message);
            return true;
        }

        public virtual void Execute(object message)
        {
            var inputType = message.GetType();
            _request.Set(inputType, message);

            var chain = _resolver.FindUniqueByType(inputType);

            if (chain == null)
            {
                throw new NoHandlerException(inputType);
            }

            try
            {
                _factory.BuildPartial(chain).InvokePartial();
                _logger.DebugMessage(() => new InlineMessageProcessed
                {
                    Envelope = _envelope,
                    Message = message
                });
            }
            catch (Exception e)
            {
                _logger.Error(_envelope.CorrelationId, "Failed processing inline message " + message, e);
                throw;
            }

            _request.Clear(inputType);
        }
    }

    public class InlineMessageProcessed : MessageLogRecord
    {
        public object Message { get; set; }
        public Envelope Envelope { get; set; }
        public override MessageRecord ToRecord()
        {
            return new MessageRecord()
            {
                Message = "Inline message processed: " + Message,
                Id = Envelope.CorrelationId,
                ParentId = Envelope.ParentId,
                Headers = "{0}={1}".ToFormat(Envelope.MessageTypeKey, Message.GetType().FullName)
            };
        }
    }
}