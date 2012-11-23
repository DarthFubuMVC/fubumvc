using AspNetApplication;
using FubuMVC.Core.Http;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.AspNetTesting
{
    [TestFixture]
    public class reading_request_headers
    {
        [Test]
        public void read_custom_header()
        {
            TestApplication.Endpoints.GetByInput(new HeaderRequest{
                Name = "x-1"
            }, configure: req => req.Headers["x-1"] = "A").ReadAsText().ShouldEqual("A");
        }

        [Test]
        public void read_build_in_header()
        {
            TestApplication.Endpoints.GetByInput(new HeaderRequest
            {
                Name = HttpRequestHeaders.IfNoneMatch
            }, configure: req => req.Headers[HttpRequestHeaders.IfNoneMatch] = "A").ReadAsText().ShouldEqual("A");
        }
    }
}