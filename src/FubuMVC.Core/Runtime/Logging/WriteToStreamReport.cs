namespace FubuMVC.Core.Runtime.Logging
{
    // TODO -- write out the stream contents?
    public class WriteToStreamReport : LogRecord
    {
        private readonly string _contentType;

        public WriteToStreamReport(string contentType)
        {
            _contentType = contentType;
        }

        public string ContentType
        {
            get { return _contentType; }
        }

        public bool Equals(WriteToStreamReport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._contentType, _contentType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (WriteToStreamReport)) return false;
            return Equals((WriteToStreamReport) obj);
        }

        public override int GetHashCode()
        {
            return (_contentType != null ? _contentType.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("ContentType: {0}", _contentType);
        }
    }
}