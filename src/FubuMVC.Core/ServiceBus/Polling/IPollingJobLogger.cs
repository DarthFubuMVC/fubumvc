using System;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public interface IPollingJobLogger
    {
        void Stopping(Type jobType);
        void Starting(Guid id, IJob job);
        void Successful(Guid id, IJob job);
        void Failed(Guid id, IJob job, Exception ex);
        void FailedToSchedule(Type jobType, Exception exception);
    }
}