using System;

namespace FubuMVC.Diagnostics.Chains
{
    public class ChainDetailsRequest
    {
        public Guid Id { get; set; }

        public bool Equals(ChainDetailsRequest other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id.Equals(Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ChainDetailsRequest)) return false;
            return Equals((ChainDetailsRequest) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Id: {0}", Id);
        }
    }
}