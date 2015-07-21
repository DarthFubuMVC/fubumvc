using System;
using System.Threading.Tasks;
using FubuCore.Reflection;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Runtime.Routing;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs
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