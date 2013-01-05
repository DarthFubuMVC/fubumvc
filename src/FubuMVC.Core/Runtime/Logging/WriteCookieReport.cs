using System;
using System.Web;
using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuCore;
using Cookie = FubuMVC.Core.Http.Cookies.Cookie;

namespace FubuMVC.Core.Runtime.Logging
{
    public class WriteCookieReport : LogRecord, DescribesItself
    {
        private readonly Cookie _cookie;

        public WriteCookieReport(Cookie cookie)
        {
            _cookie = cookie;
        }

        public Cookie Cookie
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

        public void Describe(Description description)
        {
            description.Title = "Wrote cookie {0} to response".ToFormat(_cookie);
            description.ShortDescription = _cookie.ToString();
        }

        public override string ToString()
        {
            return string.Format("Cookie: {0}", _cookie);
        }
    }
}