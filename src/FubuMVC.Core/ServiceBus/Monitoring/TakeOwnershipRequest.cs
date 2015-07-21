using System;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class TakeOwnershipRequest
    {
        public TakeOwnershipRequest(Uri subject)
        {
            Subject = subject;
        }

        public TakeOwnershipRequest()
        {
        }

        public Uri Subject { get; set; }

        protected bool Equals(TakeOwnershipRequest other)
        {
            return Equals(Subject, other.Subject);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TakeOwnershipRequest) obj);
        }

        public override int GetHashCode()
        {
            return (Subject != null ? Subject.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("TakeOwnershipRequest for subject: {0}", Subject);
        }
    }
}