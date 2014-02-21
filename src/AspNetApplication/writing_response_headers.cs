using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace AspNetApplication
{
    public class ResponseHeadersEndpoint
    {
        private readonly IHttpResponse _response;
        private readonly IOutputWriter _writer;

        public ResponseHeadersEndpoint(IHttpResponse response, IOutputWriter writer)
        {
            _response = response;
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