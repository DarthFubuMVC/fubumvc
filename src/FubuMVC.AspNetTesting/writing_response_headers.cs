using AspNetApplication;
using FubuMVC.Core.Http;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.AspNetTesting
{
    [TestFixture]
    public class writing_response_headers
    {
        [Test]
        public void can_write_extension_headers()
        {
            var response = TestApplication.Endpoints.Get<ResponseHeadersEndpoint>(x => x.get_response_headers());

            response.ResponseHeaderFor("x-1").ShouldEqual("a");
            response.ResponseHeaderFor("x-2").ShouldEqual("b");
        }

        [Test]
        public void can_write_built_in_response_headers()
        {
            var response = TestApplication.Endpoints.Get<ResponseHeadersEndpoint>(x => x.get_response_headers());
            response.ResponseHeaderFor(HttpResponseHeaders.KeepAlive).ShouldEqual("True");
            response.ResponseHeaderFor(HttpResponseHeaders.Server).ShouldEndWith("Server1");
            
        }

        [Test]
        public void can_write_etag()
        {
            var response = TestApplication.Endpoints.Get<ResponseHeadersEndpoint>(x => x.get_etag());

            response.ResponseHeaderFor(HttpResponseHeaders.ETag).ShouldEqual("123456");
        }

        [Test]
        public void can_write_content_headers()
        {
            var response = TestApplication.Endpoints.Get<ResponseHeadersEndpoint>(x => x.get_content_headers());

            response.ResponseHeaderFor(HttpResponseHeaders.ContentMd5).ShouldEqual("A");
            response.ResponseHeaderFor(HttpResponseHeaders.ContentDisposition).ShouldEqual("B");
            response.ResponseHeaderFor(HttpResponseHeaders.ContentLocation).ShouldEqual("C");
            response.ResponseHeaderFor(HttpResponseHeaders.Allow).ShouldEqual("D");
            response.ResponseHeaderFor(HttpResponseHeaders.ContentEncoding).ShouldContain("UTF-16");
            response.ResponseHeaderFor(HttpResponseHeaders.ContentLength).ShouldEqual("19");
            response.ResponseHeaderFor(HttpResponseHeaders.ContentLanguage).ShouldEqual("jp-JP");
            response.ResponseHeaderFor(HttpResponseHeaders.ContentRange).ShouldEqual("E");
            response.ResponseHeaderFor(HttpResponseHeaders.Expires).ShouldEqual("5");
            response.ResponseHeaderFor(HttpResponseHeaders.LastModified).ShouldEqual("12345");

        }
    }

}