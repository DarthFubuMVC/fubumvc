using System;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class TaskOwner
    {
        public string Owner { get; set; }
        public Uri Id { get; set; }

        protected bool Equals(TaskOwner other)
        {
            return string.Equals(Owner, other.Owner) && Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TaskOwner) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Owner != null ? Owner.GetHashCode() : 0)*397) ^ (Id != null ? Id.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Owner: {0}, Id: {1}", Owner, Id);
        }
    }
}