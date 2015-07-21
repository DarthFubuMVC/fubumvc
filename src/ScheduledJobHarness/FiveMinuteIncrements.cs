using System;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;

namespace ScheduledJobHarness
{
    public class FiveMinuteIncrements : IScheduleRule
    {
        public DateTimeOffset ScheduleNextTime(DateTimeOffset currentTime, JobExecutionRecord lastExecution)
        {
            var hour = new DateTimeOffset(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, 0, 0,
                currentTime.Offset);



            while (hour < currentTime)
            {
                hour = hour.AddMinutes(5);
            }

            return hour;
        }
    }
}