using System;
using FubuCore.Reflection;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence
{
    public class JobStatus 
    {
        public static JobStatus For<T>(DateTimeOffset nextTime)
        {
            return new JobStatus(typeof (T), nextTime);
        }

        public JobStatus(Type jobType)
        {
            JobType = jobType;
        }

        public JobStatus(Type jobType, DateTimeOffset nextTime)
        {
            JobType = jobType;
            NextTime = nextTime;
        }

        public JobExecutionStatus Status { get; set; }

        public Type JobType { get; set; }
        public DateTimeOffset? NextTime { get; set; }
        public JobExecutionRecord LastExecution { get; set; }

        protected bool Equals(JobStatus other)
        {
            return string.Equals(JobType, other.JobType) && NextTime.Equals(other.NextTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((JobStatus) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((JobType != null ? JobType.GetHashCode() : 0)*397) ^ NextTime.GetHashCode();
            }
        }

        public JobStatusDTO ToDTO(string nodeName)
        {
            return new JobStatusDTO
            {
                NodeName = nodeName,
                JobKey = GetKey(JobType),
                LastExecution = LastExecution,
                NextTime = NextTime,
                Status = Status
            };
        }

        public static string GetKey(Type jobType)
        {
            return jobType.HasAttribute<JobKeyAttribute>() 
                ? jobType.GetAttribute<JobKeyAttribute>().Key 
                : jobType.Name;
        }
    }
}