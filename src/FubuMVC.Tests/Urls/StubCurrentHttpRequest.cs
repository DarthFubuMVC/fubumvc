using FubuMVC.Core.Http;
using FubuCore;

namespace FubuMVC.Tests.Urls
{
    public class StubCurrentHttpRequest : ICurrentHttpRequest
    {
        public string TheRawUrl;
        public string TheRelativeUrl;
        public string TheApplicationRoot = "http://server";
        public string TheHttpMethod = "GET";
        public string StubFullUrl = "http://server/";

        public string RawUrl()
        {
            return TheRawUrl;
        }

        public string RelativeUrl()
        {
            return TheRelativeUrl;
        }

        public string FullUrl()
        {
            return StubFullUrl;
        }

        public string ToFullUrl(string url)
        {
            return url.ToAbsoluteUrl(TheApplicationRoot);
        }

        public string HttpMethod()
        {
            return TheHttpMethod;
        }
    }
}