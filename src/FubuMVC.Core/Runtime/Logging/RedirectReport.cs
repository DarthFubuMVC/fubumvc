using System;
using FubuCore.Descriptions;
using FubuCore.Logging;

namespace FubuMVC.Core.Runtime.Logging
{
    public class RedirectReport : LogRecord, DescribesItself
    {
        public string Url { get; private set; }

        public RedirectReport(string url)
        {
            Url = url;
        }

        public bool Equals(RedirectReport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Url, Url);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(RedirectReport)) return false;
            return Equals((RedirectReport)obj);
        }

        public override int GetHashCode()
        {
            return (Url != null ? Url.GetHashCode() : 0);
        }

        public void Describe(Description description)
        {
            description.Title = "Redirected the browser to " + Url;
        }

        public override string ToString()
        {
            return string.Format("Url: {0}", Url);
        }
    }
}