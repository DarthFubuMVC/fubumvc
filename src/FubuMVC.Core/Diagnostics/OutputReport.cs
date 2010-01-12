namespace FubuMVC.Core.Diagnostics
{
    public class OutputReport : IBehaviorDetails
    {
        public string ContentType;
        public string Contents;

        public void AcceptVisitor(IBehaviorDetailsVisitor visitor)
        {
            visitor.WriteOutput(this);
        }

        public bool Equals(OutputReport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ContentType, ContentType) && Equals(other.Contents, Contents);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (OutputReport)) return false;
            return Equals((OutputReport) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ContentType != null ? ContentType.GetHashCode() : 0)*397) ^ (Contents != null ? Contents.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("ContentType: {0}, Contents: {1}", ContentType, Contents);
        }
    }
}