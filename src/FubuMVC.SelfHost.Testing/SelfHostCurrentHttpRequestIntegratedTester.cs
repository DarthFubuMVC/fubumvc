using FubuMVC.Core.Http;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.SelfHost.Testing
{
    [TestFixture]
    public class SelfHostCurrentHttpRequestIntegratedTester
    {
        [Test]
        public void read_raw_url()
        {
            var request = Harness.Endpoints.Get<HttpCurrentRequestEndpoints>(x => x.get_request_data())
                .ReadAsJson<HttpRequestData>();

            var root = Harness.Root;

            request.RawUrl.ShouldEqual(root + "/request/data");
            request.RelativeUrl.ShouldEqual("request/data");
            request.HttpMethod.ShouldEqual("GET");
            request.FullUrl.ShouldEqual(root + "/request/data");
        }
    }

    public class HttpRequestData
    {
        public string RawUrl { get; set; }
        public string RelativeUrl { get; set; }
        public string FullUrl { get; set; }
        public string HttpMethod { get; set; }
    }

    public class HttpCurrentRequestEndpoints
    {
        private readonly ICurrentHttpRequest _request;

        public HttpCurrentRequestEndpoints(ICurrentHttpRequest request)
        {
            _request = request;
        }

        public HttpRequestData post_request_data()
        {
            return get_request_data();
        }

        public HttpRequestData get_request_data()
        {
            return new HttpRequestData{
                FullUrl = _request.FullUrl(),
                HttpMethod = _request.HttpMethod(),
                RawUrl = _request.RawUrl(),
                RelativeUrl = _request.RelativeUrl()
            };
        }
    }
}