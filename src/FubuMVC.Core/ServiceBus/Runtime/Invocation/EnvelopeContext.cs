using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Dates;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public class EnvelopeContext : IEnvelopeContext
    {
        private readonly ISystemTime _systemTime;
        private readonly IChainInvoker _invoker;
        private readonly IHandlerPipeline _pipeline;

        public EnvelopeContext(ILogger logger, ISystemTime systemTime, IChainInvoker invoker, IOutgoingSender outgoing, IHandlerPipeline pipeline)
        {
            this.logger = logger;
            _systemTime = systemTime;
            _invoker = invoker;
            Outgoing = outgoing;
            _pipeline = pipeline;
        }

        protected IOutgoingSender Outgoing { get; }

        public void SendOutgoingMessages(Envelope original, IEnumerable<object> cascadingMessages)
        {
            var doNowActions = cascadingMessages.OfType<IImmediateContinuation>().SelectMany(x => x.Actions());
            var cascading = cascadingMessages.Where(x => !(x is IImmediateContinuation));

            Outgoing.SendOutgoingMessages(original, cascading);

            doNowActions.Each(message => {
                try
                {
                    _invoker.InvokeNow(message);
                }
                catch (Exception e)
                {
                    logger.Error("Failed while trying to invoke a cascading message:\n" + message.ToString(), e);
                }
            });
        }

        protected ILogger logger { get; }

        public void SendFailureAcknowledgement(Envelope original, string message)
        {
            Outgoing.SendFailureAcknowledgement(original, message);
        }

        public ISystemTime SystemTime
        {
            get { return _systemTime; }
        }

        public virtual void InfoMessage<T>(Func<T> func) where T : class, LogTopic
        {
            logger.InfoMessage(func);
        }

        public virtual void DebugMessage<T>(Func<T> func) where T : class, LogTopic
        {
            logger.DebugMessage(func);
        }

        public virtual void InfoMessage<T>(T message) where T : LogTopic
        {
            logger.InfoMessage(message);
        }

        public virtual void Error(string correlationId, string message, Exception exception)
        {
            logger.Error(correlationId, message, exception);
        }

        public void Retry(Envelope envelope)
        {
            _pipeline.Invoke(envelope, this);
        }

        public virtual void Dispose()
        {
            // Nothing
        }
    }
}