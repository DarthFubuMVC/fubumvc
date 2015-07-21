using System;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class PersistentTaskStatus
    {
        public PersistentTaskStatus()
        {
        }

        public PersistentTaskStatus(Uri subject, HealthStatus status)
        {
            Subject = subject;
            Status = status;
        }

        public Uri Subject { get; set; }
        public HealthStatus Status { get; set; }

        protected bool Equals(PersistentTaskStatus other)
        {
            return Equals(Subject, other.Subject) && Status == other.Status;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PersistentTaskStatus) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Subject != null ? Subject.GetHashCode() : 0)*397) ^ (int) Status;
            }
        }

        public override string ToString()
        {
            return string.Format("Subject: {0}, Status: {1}", Subject, Status);
        }
    }
}