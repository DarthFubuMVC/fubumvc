using System;
using FubuCore.Descriptions;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.ErrorHandling
{
    public class MoveToErrorQueueHandler<T> : IErrorHandler, DescribesItself where T : Exception
    {
        public IContinuation DetermineContinuation(Envelope envelope, Exception ex)
        {
            if (ex is T) return new MoveToErrorQueue(ex);

            return null;
        }

        public void Describe(Description description)
        {
            description.Title = "Move to error queue if the exception is " + typeof (T).Name;
        }
    }
}