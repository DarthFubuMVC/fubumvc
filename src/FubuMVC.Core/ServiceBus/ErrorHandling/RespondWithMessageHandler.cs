using System;
using FubuCore.Descriptions;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.ErrorHandling
{
    public class RespondWithMessageHandler<T> : IErrorHandler, DescribesItself where T : Exception
    {
        private readonly Func<T, Envelope, object> _messageFunc;

        public RespondWithMessageHandler(Func<Exception, Envelope, object> messageFunc)
        {
            _messageFunc = messageFunc;
        }

        public IContinuation DetermineContinuation(Envelope envelope, Exception ex)
        {
            var exception = ex as T;
            if (exception == null)
                return null;

            var message = _messageFunc(exception, envelope);
            return new RespondWithMessageContinuation(message);
        }

        public void Describe(Description description)
        {
            description.Title = "Respond to sender with a custom message if exception is " + typeof(T).Name;
        }
    }
}