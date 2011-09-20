namespace FubuMVC.Core.Diagnostics
{
    public class RequestLogEntry : IBehaviorDetails
    {
        public string Message { get; set; }

        public void AcceptVisitor(IBehaviorDetailsVisitor visitor)
        {
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RequestLogEntry)) return false;
            return Equals((RequestLogEntry) obj);
        }

        public bool Equals(RequestLogEntry other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Message, Message);
        }

        public override int GetHashCode()
        {
            return Message.GetHashCode();
        }
    }
}