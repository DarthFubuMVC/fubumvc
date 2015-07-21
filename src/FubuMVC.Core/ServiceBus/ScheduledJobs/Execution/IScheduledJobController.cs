using FubuMVC.Core.ServiceBus.Polling;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Execution
{
    public interface IScheduledJobController
    {
        void Activate();
        bool IsActive();
        void Deactivate();
        void Reschedule<T>(RescheduleRequest<T> request) where T : IJob;
        void PerformHealthChecks();

        void ExecuteNow(IScheduledJob job);
    }
}