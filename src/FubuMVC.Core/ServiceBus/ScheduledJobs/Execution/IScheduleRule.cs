using System;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Execution
{
    public interface IScheduleRule
    {
        DateTimeOffset ScheduleNextTime(DateTimeOffset currentTime, JobExecutionRecord lastExecution);
    }
}