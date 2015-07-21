using System;
using FubuMVC.Core.ServiceBus.ErrorHandling;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public interface IMessageCallback
    {
        void MarkSuccessful();
        void MarkFailed(Exception ex);

        void MoveToDelayedUntil(DateTime time);
        void MoveToErrors(ErrorReport report);
        void Requeue();
    }
}