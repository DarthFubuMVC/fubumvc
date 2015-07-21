using System;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class TaskDeactivation
    {
        public TaskDeactivation()
        {
        }

        public TaskDeactivation(Uri subject)
        {
            Subject = subject;
        }

        public Uri Subject { get; set; }

        protected bool Equals(TaskDeactivation other)
        {
            return Equals(Subject, other.Subject);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TaskDeactivation) obj);
        }

        public override int GetHashCode()
        {
            return (Subject != null ? Subject.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Deactivate Task: {0}", Subject);
        }
    }
}