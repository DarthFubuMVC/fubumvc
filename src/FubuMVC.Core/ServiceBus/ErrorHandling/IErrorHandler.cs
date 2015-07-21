using System;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.ErrorHandling
{
    public interface IErrorHandler
    {
        IContinuation DetermineContinuation(Envelope envelope, Exception ex);
    }
}