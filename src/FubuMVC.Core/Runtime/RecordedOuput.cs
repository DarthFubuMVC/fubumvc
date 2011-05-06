namespace FubuMVC.Core.Runtime
{
    public class RecordedOuput
    {
        public RecordedOuput(string recordedContentType, string recordedOutput)
        {
            RecordedOutput = recordedOutput;
            RecordedContentType = recordedContentType;
        }

        public string RecordedOutput { get; private set; }
        public string RecordedContentType { get; private set; }

        public bool Equals(RecordedOuput other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.RecordedOutput, RecordedOutput) && Equals(other.RecordedContentType, RecordedContentType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RecordedOuput)) return false;
            return Equals((RecordedOuput) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((RecordedOutput != null ? RecordedOutput.GetHashCode() : 0)*397) ^ (RecordedContentType != null ? RecordedContentType.GetHashCode() : 0);
            }
        }
    }
}