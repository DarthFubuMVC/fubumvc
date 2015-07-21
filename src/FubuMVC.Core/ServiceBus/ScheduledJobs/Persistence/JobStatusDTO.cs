using System;
using FubuCore;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence
{
    public class JobStatusDTO
    {
        public static string ToId(string node, string jobKey)
        {
            return node + "/" + jobKey;
        }

        public string JobKey { get; set; }

        public string Id
        {
            get
            {
                return ToId(NodeName, JobKey);
            }
        }

        public DateTimeOffset? NextTime { get; set; }
        public JobExecutionRecord LastExecution { get; set; }
        public string NodeName { get; set; }
        public JobExecutionStatus Status { get; set; }
        public string Executor { get; set; }

        public DateTimeOffset? Started { get; set; }

        protected bool Equals(JobStatusDTO other)
        {
            return string.Equals(NodeName, other.NodeName) && string.Equals(JobKey, other.JobKey);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((JobStatusDTO) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((NodeName != null ? NodeName.GetHashCode() : 0)*397) ^ (JobKey != null ? JobKey.GetHashCode() : 0);
            }
        }

        public string GetStatusDescription()
        {
            if (Executor.IsNotEmpty()) return "{0} on Node '{1}'".ToFormat(Status, Executor);

            return Status.ToString();
        }

        public string GetLastExecutionDescription()
        {
            if (LastExecution == null) return "None";

            return LastExecution.ToString();
        }
    }
}