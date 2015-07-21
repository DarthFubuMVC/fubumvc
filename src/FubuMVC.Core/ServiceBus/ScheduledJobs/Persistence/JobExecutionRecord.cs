using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence
{
    public class JobExecutionRecord
    {
        public const string ExceptionSeparator = "\n-----------------------------\n";
        public long Duration { get; set; }
        public DateTimeOffset Finished { get; set; }
        public bool Success { get; set; }
        public string ExceptionText { get; set; }
        public int Attempts { get; set; }
        public string Executor { get; set; }


        public override string ToString()
        {
            if (Success)
            {
                return "Succeeded at {0} on node '{1}' in {2} milliseconds"
                    .ToFormat(Finished.ToLocalTime(), Executor, Duration);
            }

            return "Failed at {0} on node {1} in {2} milliseconds"
                .ToFormat(Finished.ToLocalTime(), Executor, Duration);
        }

        protected bool Equals(JobExecutionRecord other)
        {
            return Duration.Equals(other.Duration) && Finished.Equals(other.Finished) && Success.Equals(other.Success);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((JobExecutionRecord) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Duration.GetHashCode();
                hashCode = (hashCode*397) ^ Finished.GetHashCode();
                hashCode = (hashCode*397) ^ Success.GetHashCode();
                return hashCode;
            }
        }

        public void ReadException(Exception exception)
        {
            if (exception is AggregateException)
            {
                ExceptionText = exception.As<AggregateException>()
                    .InnerExceptions.Select(x => x.ToString())
                    .Join(ExceptionSeparator);
            }
            else
            {
                ExceptionText = exception.ToString();
            }
        }
    }
}