using System;

namespace TestMessages.ScenarioSupport
{
    public class MirrorMessage<T>
    {
        public Guid Id { get; set; }

        protected bool Equals(MirrorMessage<T> other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MirrorMessage<T>) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}