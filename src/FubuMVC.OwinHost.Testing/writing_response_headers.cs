using System.Threading;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class writing_response_headers
    {
        [Test]
        public void can_write_extension_headers()
        {
            var response = Harness.Endpoints.Get<ResponseHeadersEndpoint>(x => x.get_response_headers());

            response.ResponseHeaderFor("x-1").ShouldEqual("a");
            response.ResponseHeaderFor("x-2").ShouldEqual("b");
        }

        [Test]
        public void can_write_built_in_response_headers()
        {
            Thread.Sleep(100); //mono ci build hack, maybe static state in Harness?
            var response = Harness.Endpoints.Get<ResponseHeadersEndpoint>(x => x.get_response_headers());
            response.ResponseHeaderFor(HttpResponseHeaders.KeepAlive).ShouldEqual("True");
            response.ResponseHeaderFor(HttpResponseHeaders.Server).ShouldStartWith("Server1");
        }

        [Test]
        public void can_write_etag()
        {
            var response = Harness.Endpoints.Get<ResponseHeadersEndpoint>(x => x.get_etag());

            response.ResponseHeaderFor(HttpResponseHeaders.ETag).ShouldEqual("123456");
        }

        [Test]
        public void can_write_content_headers()
        {
            var response = Harness.Endpoints.Get<ResponseHeadersEndpoint>(x => x.get_content_headers());

            response.ResponseHeaderFor(HttpResponseHeaders.ContentMd5).ShouldEqual("A");
            response.ResponseHeaderFor(HttpResponseHeaders.ContentDisposition).ShouldEqual("B");
            response.ResponseHeaderFor(HttpResponseHeaders.ContentLocation).ShouldEqual("C");
            response.ResponseHeaderFor(HttpResponseHeaders.Allow).ShouldEqual("D");
            response.ResponseHeaderFor(HttpResponseHeaders.ContentEncoding).ShouldEqual("UTF-16");
            response.ResponseHeaderFor(HttpResponseHeaders.ContentLength).ShouldEqual("19");
            response.ResponseHeaderFor(HttpResponseHeaders.ContentLanguage).ShouldEqual("jp-JP");
            response.ResponseHeaderFor(HttpResponseHeaders.ContentRange).ShouldEqual("E");
            response.ResponseHeaderFor(HttpResponseHeaders.Expires).ShouldEqual("5");
            response.ResponseHeaderFor(HttpResponseHeaders.LastModified).ShouldEqual("12345");

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

        public string get_etag()
        {
            _writer.AppendHeader(HttpResponseHeaders.ETag, "123456");

            return "Nothing to see here";
        }
    }
}