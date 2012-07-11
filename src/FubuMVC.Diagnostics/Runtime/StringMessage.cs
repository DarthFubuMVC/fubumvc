namespace FubuMVC.Diagnostics.Runtime
{
    public class StringMessage
    {
        public string Message { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (StringMessage)) return false;
            return Equals((StringMessage) obj);
        }

        public bool Equals(StringMessage other)
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