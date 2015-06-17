using System;
using System.Threading.Tasks;
using FubuCore.Reflection;
using FubuTransportation.Polling;
using FubuTransportation.Runtime.Routing;
using FubuTransportation.ScheduledJobs.Execution;
using FubuTransportation.ScheduledJobs.Persistence;

namespace FubuTransportation.ScheduledJobs
{
    public interface IScheduledJob
    {
        Type JobType { get; }
        void Initialize(DateTimeOffset now, IJobExecutor executor, JobSchedule schedule);
        Accessor Channel { get; set; }
        TimeSpan Timeout { get;}
        IRoutingRule ToRoutingRule();
        bool ShouldReschedule(DateTimeOffset now, IJobTimer timer);

        void Execute(IJobExecutor executor);
    }

    public interface IScheduledJob<T> : IScheduledJob where T : IJob
    {
        Task<RescheduleRequest<T>> ToTask(IJob job, IJobRunTracker tracker);
    }
}