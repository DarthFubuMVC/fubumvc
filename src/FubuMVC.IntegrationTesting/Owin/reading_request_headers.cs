using System.Linq;
using FubuMVC.Core.Http;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class reading_request_headers
    {
        [Test]
        public void read_build_in_header()
        {
            var message = new HeaderRequest
            {
                Name = HttpRequestHeaders.IfNoneMatch
            };

            TestHost.Scenario(_ =>
            {
                _.Get.Input(message);
                _.Request.AppendHeader(HttpRequestHeaders.IfNoneMatch, "A");

                _.ContentShouldBe("A");
            });
        }

        [Test]
        public void read_custom_header()
        {
            var message = new HeaderRequest
            {
                Name = "x-1"
            };

            TestHost.Scenario(_ =>
            {
                _.Get.Input(message);
                _.Request.AppendHeader("x-1", "A");

                _.ContentShouldBe("A");
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