namespace FubuMVC.Core.Runtime
{
    public class RecordedOutput
    {
        public RecordedOutput(string recordedContentType, string recordedOutput)
        {
            Content = recordedOutput;
            ContentType = recordedContentType;
        }

        public string Content { get; private set; }
        public string ContentType { get; private set; }

        public bool Equals(RecordedOutput other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Content, Content) && Equals(other.ContentType, ContentType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RecordedOutput)) return false;
            return Equals((RecordedOutput) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Content != null ? Content.GetHashCode() : 0)*397) ^ (ContentType != null ? ContentType.GetHashCode() : 0);
            }
        }
    }
}