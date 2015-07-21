using System;
using FubuCore;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;

namespace ScheduledJobHarness
{
    public class AtSecondsAfterTheMinute : IScheduleRule
    {
        private readonly int _seconds;

        public AtSecondsAfterTheMinute(int seconds)
        {
            _seconds = seconds;
        }

        public DateTimeOffset ScheduleNextTime(DateTimeOffset currentTime, JobExecutionRecord lastExecution)
        {
            var next = currentTime.Subtract(new TimeSpan(0, 0, 0, currentTime.Second, currentTime.Millisecond));

            while (next < currentTime)
            {
                next = next.Add(_seconds.Seconds());
            }

            return next;
        }
    }
}