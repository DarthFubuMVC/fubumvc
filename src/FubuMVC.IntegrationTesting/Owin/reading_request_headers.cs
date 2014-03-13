using System.Linq;
using FubuMVC.Core.Http;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class reading_request_headers
    {
        [Test]
        public void read_build_in_header()
        {
            HarnessApplication.Run(x => {
                x.GetByInput(new HeaderRequest
                {
                    Name = HttpRequestHeaders.IfNoneMatch
                }, configure: req => req.Headers[HttpRequestHeaders.IfNoneMatch] = "A").ReadAsText().ShouldEqual("A");
            });
        }

        [Test]
        public void read_custom_header()
        {
            HarnessApplication.Run(x => {
                x.GetByInput(new HeaderRequest
                {
                    Name = "x-1"
                }, configure: req => req.Headers["x-1"] = "A").ReadAsText().ShouldEqual("A");
            });
        }
    }

    public class RequestHeadersEndpoint
    {
        private readonly IHttpRequest _headers;

        public RequestHeadersEndpoint(IHttpRequest headers)
        {
            _headers = headers;
        }

        public string get_header_Name(HeaderRequest request)
        {
            return _headers.GetHeader(request.Name).FirstOrDefault();
        }
    }

    public class HeaderRequest
    {
        public string Name { get; set; }
    }
}