using FubuMVC.Core.Http;

namespace FubuMVC.Tests.Urls
{
    public class StubCurrentHttpRequest : ICurrentHttpRequest
    {
        public string TheRawUrl;
        public string TheRelativeUrl;
        public string TheApplicationRoot = "http://server";
        public string TheHttpMethod = "GET";

        public string RawUrl()
        {
            return TheRawUrl;
        }

        public string RelativeUrl()
        {
            return TheRelativeUrl;
        }

        public string ApplicationRoot()
        {
            return TheApplicationRoot;
        }

        public string HttpMethod()
        {
            return TheHttpMethod;
        }
    }
}