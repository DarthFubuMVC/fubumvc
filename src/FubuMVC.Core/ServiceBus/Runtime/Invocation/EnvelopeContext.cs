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
        private readonly ILogger _logger;
        private readonly ISystemTime _systemTime;
        private readonly IChainInvoker _invoker;
        private readonly IOutgoingSender _outgoing;

        public EnvelopeContext(ILogger logger, ISystemTime systemTime, IChainInvoker invoker, IOutgoingSender outgoing)
        {
            _logger = logger;
            _systemTime = systemTime;
            _invoker = invoker;
            _outgoing = outgoing;
        }

        protected IOutgoingSender Outgoing
        {
            get { return _outgoing; }
        }

        // virtual for testing, setter to avoid bi-directional dependency problems
        public virtual IHandlerPipeline Pipeline { get; set; }

        public void SendOutgoingMessages(Envelope original, IEnumerable<object> cascadingMessages)
        {
            var doNowActions = cascadingMessages.OfType<IImmediateContinuation>().SelectMany(x => x.Actions());
            var cascading = cascadingMessages.Where(x => !(x is IImmediateContinuation));

            _outgoing.SendOutgoingMessages(original, cascading);

            doNowActions.Each(message => {
                try
                {
                    _invoker.InvokeNow(message);
                }
                catch (Exception e)
                {
                    _logger.Error("Failed while trying to invoke a cascading message:\n" + message.ToString(), e);
                }
            });
        }

        protected ILogger logger
        {
            get { return _logger; }
        }

        public void SendFailureAcknowledgement(Envelope original, string message)
        {
            _outgoing.SendFailureAcknowledgement(original, message);
        }

        public ISystemTime SystemTime
        {
            get { return _systemTime; }
        }

        public virtual void InfoMessage<T>(Func<T> func) where T : class, LogTopic
        {
            _logger.InfoMessage(func);
        }

        public virtual void DebugMessage<T>(Func<T> func) where T : class, LogTopic
        {
            _logger.DebugMessage(func);
        }

        public virtual void InfoMessage<T>(T message) where T : LogTopic
        {
            _logger.InfoMessage(message);
        }

        public virtual void Error(string correlationId, string message, Exception exception)
        {
            _logger.Error(correlationId, message, exception);
        }

        public virtual void Retry(Envelope envelope)
        {
            Pipeline.Invoke(envelope);
        }

        public virtual void Dispose()
        {
            // Nothing
        }
    }
}