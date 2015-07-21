using System;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Execution
{
    public abstract class ScheduledJobRecord : LogRecord
    {
        public ScheduledJobRecord(IJob job)
        {
            JobKey = JobStatus.GetKey(job.GetType());
        }

        public string JobKey { get; set; }
        public string NodeId { get; set; }
    }

    public class ScheduledJobScheduled : LogRecord
    {
        public ScheduledJobScheduled(Type jobType, DateTimeOffset next)
        {
            JobKey = JobStatus.GetKey(jobType);
            ScheduledTime = next;
        }

        public string JobKey { get; set; }
        public DateTimeOffset ScheduledTime { get; set; }

        public override string ToString()
        {
            return "Scheduled job {0} Scheduled to start at {1}"
                .ToFormat(JobKey, ScheduledTime.ToLocalTime());
        }
    }



    public class ScheduledJobStarted : ScheduledJobRecord
    {
        public ScheduledJobStarted(IJob job) : base(job)
        {
        }

        public override string ToString()
        {
            return "Scheduled job {0} started on node {1} at {2}".ToFormat(JobKey, NodeId, Time.ToLocalTime());
        }
    }

    public class ScheduledJobSucceeded : ScheduledJobRecord
    {
        public ScheduledJobSucceeded(IJob job) : base(job)
        {
        }

        public override string ToString()
        {
            return "Scheduled job {0} succeeded on Node {1} at {2}".ToFormat(JobKey, NodeId, Time.ToLocalTime());
        }
    }

    public class ScheduledJobFailed : ScheduledJobRecord
    {
        public ScheduledJobFailed(IJob job, Exception ex) : base(job)
        {
            Exception = ex;
        }

        public Exception Exception { get; set; }

        public override string ToString()
        {
            return "Scheduled job {0} failed with exception {1} on node {2} at {3}"
                .ToFormat(JobKey, Exception, NodeId, Time.ToLocalTime());
        }
    }

}