using System;
using FubuTransportation.Polling;

namespace FubuTransportation.ScheduledJobs.Execution
{
    public interface IJobExecutor
    {
        void Execute<T>(TimeSpan timeout) where T : IJob;
        void Schedule<T>(IScheduledJob<T> job, DateTimeOffset nextTime) where T : IJob;

    }
}