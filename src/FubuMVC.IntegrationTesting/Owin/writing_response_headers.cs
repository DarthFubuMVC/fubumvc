using System.Threading;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class writing_response_headers
    {
        [Test]
        public void can_write_extension_headers()
        {
            TestHost.Scenario(_ => {
                _.Get.Action<ResponseHeadersEndpoint>(x => x.get_response_headers());

                _.Header("x-1").SingleValueShouldEqual("a");
                _.Header("x-2").SingleValueShouldEqual("b");
            });
        }


        [Test]
        public void can_write_built_in_response_headers()
        {
            TestHost.Scenario(_ => {
                _.Get.Action<ResponseHeadersEndpoint>(x => x.get_response_headers());

                _.Header(HttpResponseHeaders.KeepAlive).SingleValueShouldEqual("True");
                _.Header(HttpResponseHeaders.Server).SingleValueShouldEqual("Server1");

            });

        }

        [Test]
        public void can_write_etag()
        {
            TestHost.Scenario(_ => {
                _.Get.Action<ResponseHeadersEndpoint>(x => x.get_etag());

                _.Header(HttpResponseHeaders.ETag).SingleValueShouldEqual("123456");
            });
        }

        [Test]
        public void can_write_content_headers()
        {
            TestHost.Scenario(_ => {
                _.Get.Action<ResponseHeadersEndpoint>(x => x.get_content_headers());

                _.Header(HttpResponseHeaders.ContentMd5).SingleValueShouldEqual("A");
                _.Header(HttpResponseHeaders.ContentDisposition).SingleValueShouldEqual("B");
                _.Header(HttpResponseHeaders.ContentLocation).SingleValueShouldEqual("C");
                _.Header(HttpResponseHeaders.Allow).SingleValueShouldEqual("D");
                _.Header(HttpResponseHeaders.ContentEncoding).SingleValueShouldEqual("UTF-16");
                _.Header(HttpResponseHeaders.ContentLength).SingleValueShouldEqual("19");
                _.Header(HttpResponseHeaders.ContentLanguage).SingleValueShouldEqual("jp-JP");
                _.Header(HttpResponseHeaders.ContentRange).SingleValueShouldEqual("E");
                _.Header(HttpResponseHeaders.Expires).SingleValueShouldEqual("5");
                _.Header(HttpResponseHeaders.LastModified).SingleValueShouldEqual("12345");
            });
        }

        [Test]
        public void can_write_multiple_cookies()
        {
            TestHost.Scenario(_ => {
                _.Get.Action<ResponseHeadersEndpoint>(x => x.get_multiple_cookies());

                _.Response.CookieFor("Foo").Value.ShouldEqual("1");
                _.Response.CookieFor("Foo").Value.ShouldEqual("1");
                
            });
        }
    }

    public class ResponseHeadersEndpoint
    {
        private readonly IOutputWriter _writer;

        public ResponseHeadersEndpoint(IOutputWriter writer)
        {
            _writer = writer;
        }

        public string get_response_headers()
        {
            _writer.AppendHeader("x-1", "a");
            _writer.AppendHeader("x-2", "b");

            _writer.AppendHeader(HttpResponseHeaders.KeepAlive, true.ToString());
            _writer.AppendHeader(HttpResponseHeaders.Server, "Server1");

            return "Nothing to see here";
        }

        public string get_content_headers()
        {
            _writer.AppendHeader(HttpResponseHeaders.ContentMd5, "A");
            _writer.AppendHeader(HttpResponseHeaders.ContentDisposition, "B");
            _writer.AppendHeader(HttpResponseHeaders.ContentLocation, "C");
            _writer.AppendHeader(HttpResponseHeaders.Allow, "D");
            _writer.AppendHeader(HttpResponseHeaders.ContentEncoding, "UTF-16");
            _writer.AppendHeader(HttpResponseHeaders.ContentLength, "Nothing to see here".Length.ToString());
            _writer.AppendHeader(HttpResponseHeaders.ContentLanguage, "jp-JP");
            _writer.AppendHeader(HttpResponseHeaders.ContentRange, "E");
            _writer.AppendHeader(HttpResponseHeaders.Expires, "5");
            _writer.AppendHeader(HttpResponseHeaders.LastModified, "12345");


            return "Nothing to see here";
        }

        public string get_multiple_cookies()
        {
            _writer.AppendCookie(new Cookie("Foo", "1"));
            _writer.AppendCookie(new Cookie("Bar", "2"));

            return "Just look at the cookies";
        }

        public string get_etag()
        {
            _writer.AppendHeader(HttpResponseHeaders.ETag, "123456");

            return "Nothing to see here";
        }
    }
}