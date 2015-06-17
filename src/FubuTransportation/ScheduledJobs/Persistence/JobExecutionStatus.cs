namespace FubuTransportation.ScheduledJobs.Persistence
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