using FubuMVC.Core.Http;

namespace FubuMVC.Tests.Urls
{
    public class StubCurrentRequest : ICurrentRequest
    {
        public string TheRawUrl;
        public string TheRelativeUrl;
        public string TheApplicationRoot;
        public string TheHttpMethod;

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