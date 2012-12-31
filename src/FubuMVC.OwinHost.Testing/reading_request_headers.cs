using FubuMVC.Core.Http;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class reading_request_headers
    {
        [Test]
        public void read_custom_header()
        {
            Harness.Endpoints.GetByInput(new HeaderRequest{
                Name = "x-1"
            }, configure: req => req.Headers["x-1"] = "A").ReadAsText().ShouldEqual("A");
        }

        [Test]
        public void read_build_in_header()
        {
            Harness.Endpoints.GetByInput(new HeaderRequest
            {
                Name = HttpRequestHeaders.IfNoneMatch
            }, configure: req => req.Headers[HttpRequestHeaders.IfNoneMatch] = "A").ReadAsText().ShouldEqual("A");
        }
    }

    public class RequestHeadersEndpoint
    {
        private readonly IRequestHeaders _headers;

        public RequestHeadersEndpoint(IRequestHeaders headers)
        {
            _headers = headers;
        }

        public string get_header_Name(HeaderRequest request)
        {
            string text = null;
            _headers.Value<string>(request.Name, x => text = x);

            return text;
        }
    }

    public class HeaderRequest
    {
        public string Name { get; set; }
    }
}