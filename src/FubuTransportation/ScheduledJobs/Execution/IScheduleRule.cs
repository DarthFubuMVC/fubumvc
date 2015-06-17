using System;
using FubuTransportation.ScheduledJobs.Persistence;

namespace FubuTransportation.ScheduledJobs.Execution
{
    public interface IScheduleRule
    {
        DateTimeOffset ScheduleNextTime(DateTimeOffset currentTime, JobExecutionRecord lastExecution);
    }
}