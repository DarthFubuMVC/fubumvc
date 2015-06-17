using FubuTransportation.Polling;

namespace FubuTransportation.ScheduledJobs.Execution
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