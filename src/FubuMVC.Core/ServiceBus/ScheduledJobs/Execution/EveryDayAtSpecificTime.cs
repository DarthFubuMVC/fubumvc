using System;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Execution
{
    public class EveryDayAtSpecificTime : IScheduleRule
    {
        private readonly int _hour;
        private readonly int _minute;

        public EveryDayAtSpecificTime(int hour, int minute)
        {
            _hour = hour;
            _minute = minute;
        }

        public DateTimeOffset ScheduleNextTime(DateTimeOffset currentTime, JobExecutionRecord lastExecution)
        {
            var localTime = currentTime.ToLocalTime();
            var oneAmToday = new DateTime(localTime.Year, localTime.Month, localTime.Day, _hour, _minute, 0, 0, DateTimeKind.Local);
            var nextScheduledTime = oneAmToday;

            if (localTime.Hour > _hour || (localTime.Hour == _hour && localTime.Minute >= _minute))
            {
                // Switch to tomorrow
                nextScheduledTime = oneAmToday.AddDays(1);
            }

            return nextScheduledTime.ToUniversalTime();
        }
    }

    /*
    public class EveryDayAt1Am : EveryDayAtSpecificTime
    {
        public EveryDayAt1Am() : base(hour: 01, minute: 00)
        {}
    }
    */
}