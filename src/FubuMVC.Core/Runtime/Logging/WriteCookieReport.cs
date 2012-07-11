using System.Web;

namespace FubuMVC.Core.Runtime.Logging
{
    public class WriteCookieReport : LogRecord
    {
        private readonly HttpCookie _cookie;

        public WriteCookieReport(HttpCookie cookie)
        {
            _cookie = cookie;
        }

        public HttpCookie Cookie
        {
            get { return _cookie; }
        }

        public bool Equals(WriteCookieReport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._cookie, _cookie);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(WriteCookieReport)) return false;
            return Equals((WriteCookieReport)obj);
        }

        public override int GetHashCode()
        {
            return (_cookie != null ? _cookie.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Cookie: {0}", _cookie);
        }
    }
}