using System;
using FubuCore;
using FubuMVC.Core.ServiceBus.Polling;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs
{
    public class RescheduleRequest<T> where T : IJob
    {
        public DateTimeOffset NextTime { get; set; }

        protected bool Equals(RescheduleRequest<T> other)
        {
            return NextTime.Equals(other.NextTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RescheduleRequest<T>) obj);
        }

        public override int GetHashCode()
        {
            return NextTime.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Reschedule job {0} to {1}", typeof(T).GetFullName(), NextTime);
        }
    }
}