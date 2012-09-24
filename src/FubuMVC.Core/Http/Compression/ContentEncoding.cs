using System;

namespace FubuMVC.Core.Http.Compression
{
    public class ContentEncoding
    {
        public static ContentEncoding GZip = new ContentEncoding("gzip");
        public static ContentEncoding Deflate = new ContentEncoding("deflate");
        public static ContentEncoding None = new ContentEncoding("None");

        private readonly string _encoding;

        private ContentEncoding(string encoding)
        {
            _encoding = encoding;
        }

        public string Value
        {
            get { return _encoding; }
        }

        public bool Matches(string encoding)
        {
            return Value.Equals(encoding, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ContentEncoding)) return false;
            return Equals((ContentEncoding) obj);
        }

        public bool Equals(ContentEncoding other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._encoding, _encoding);
        }

        public override int GetHashCode()
        {
            return _encoding.GetHashCode();
        }
    }
}