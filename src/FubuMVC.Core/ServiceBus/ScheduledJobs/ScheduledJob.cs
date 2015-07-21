using System;
using System.ComponentModel;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Runtime.Routing;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Configuration;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs
{
    public class ScheduledJob<T> : IScheduledJob<T>, DescribesItself where T : IJob
    {
        public ScheduledJob(IScheduleRule scheduler)
        {
            Scheduler = scheduler;
            Timeout = 5.Minutes();
            MaximumTimeBeforeRescheduling = 15.Minutes();
        }

        public TimeSpan MaximumTimeBeforeRescheduling { get; set; }

        // This will be completely tested through integration
        // tests only
        void IScheduledJob.Execute(IJobExecutor executor)
        {
            executor.Execute<T>(Timeout);
        }

        public Task<RescheduleRequest<T>> ToTask(IJob job, IJobRunTracker tracker)
        {
            var timeout = new JobTimeout(Timeout);
            return timeout.Execute(job).ContinueWith(t => {
                if (t.IsFaulted)
                {
                    tracker.Failure(t.Exception);
                    throw t.Exception;
                }
                
                var nextTime = Scheduler.ScheduleNextTime(tracker.Now(), LastExecution);
                tracker.Success(nextTime);

                return new RescheduleRequest<T>
                {
                    NextTime = nextTime
                };
            });
        }

        public Accessor Channel { get; set; }
        public TimeSpan Timeout { get; set; }

        IRoutingRule IScheduledJob.ToRoutingRule()
        {
            return new ScheduledJobRoutingRule<T>();
        }

        public Type JobType
        {
            get { return typeof (T); }
        }

        public IScheduleRule Scheduler { get; private set; }
        public JobExecutionRecord LastExecution { get; set; }

        void IScheduledJob.Initialize(DateTimeOffset now, IJobExecutor executor, JobSchedule schedule)
        {
            var status = schedule.Find(JobType);
            LastExecution = status.LastExecution;

            var next = Scheduler.ScheduleNextTime(now, LastExecution);

            schedule.Schedule(JobType, next);

            executor.Schedule(this, next);
        }

        public bool ShouldReschedule(DateTimeOffset now, IJobTimer timer)
        {
            var execution = timer.StatusFor(typeof (T));
            if (execution == null) return true;

            if (execution.Status == JobExecutionStatus.Scheduled)
            {
                return false;
            }

            var age = now.Subtract(execution.ExpectedTime);
            return age > MaximumTimeBeforeRescheduling;
        }

        public void Describe(Description description)
        {
            description.Title = "Job Type: " + typeof (T).GetFullName();
            description.ShortDescription = typeof (T).GetFullName();
            typeof(T).ForAttribute<DescriptionAttribute>(_ => {
                description.ShortDescription = _.Description;
            });

            if (Channel != null) description.Properties["Channel"] = Channel.Name;
            description.Properties["Maximum Time Before Rescheduling"] = MaximumTimeBeforeRescheduling.ToString();
            description.AddChild("Scheduler", Scheduler);
        }
    }
}