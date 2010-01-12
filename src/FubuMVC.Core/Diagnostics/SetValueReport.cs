using System;

namespace FubuMVC.Core.Diagnostics
{
    public class SetValueReport : IBehaviorDetails
    {
        public Type Type { get; set; }
        public object Value { get; set; }

        public void AcceptVisitor(IBehaviorDetailsVisitor visitor)
        {
            visitor.SetValue(this);
        }

        public bool Equals(SetValueReport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Type, Type) && Equals(other.Value, Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (SetValueReport)) return false;
            return Equals((SetValueReport) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0)*397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Type: {0}, Value: {1}", Type, Value);
        }
    }
}