using System;
using FubuCore.Descriptions;
using FubuCore.Logging;

namespace FubuMVC.Core.Runtime.Logging
{
    public class OutputReport : LogRecord, IHaveContentType, DescribesItself
    {
        public string ContentType { get; private set; }
        public string Contents { get; private set; }

        public OutputReport(string contents)
        {
            Contents = contents;
        }

        public OutputReport(string contentType, string contents)
        {
            ContentType = contentType;
            Contents = contents;
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
            if (obj.GetType() != typeof(OutputReport)) return false;
            return Equals((OutputReport)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ContentType != null ? ContentType.GetHashCode() : 0) * 397) ^ (Contents != null ? Contents.GetHashCode() : 0);
            }
        }

        public void Describe(Description description)
        {
            description.Title = "Output as " + ContentType;
            description.LongDescription = Contents;
        }

        public override string ToString()
        {
            return string.Format("ContentType: {0}, Contents: {1}", ContentType, Contents);
        }
    }
}