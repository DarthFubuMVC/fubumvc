namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence
{
    public enum JobExecutionStatus
    {
        Scheduled,
        Executing,
        Completed,
        Inactive,
        Failed
    }
}