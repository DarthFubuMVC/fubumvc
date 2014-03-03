using System.Net;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.Http.Owin.Middleware.StaticFiles
{
    public class WriteFileHeadContinuation : WriterContinuation
    {
        private readonly IFubuFile _file;
        private readonly HttpStatusCode _status;

        public WriteFileHeadContinuation(IHttpResponse response, IFubuFile file, HttpStatusCode status) : base(response, DoNext.Stop)
        {
            _file = file;
            _status = status;
        }

        public IFubuFile File
        {
            get { return _file; }
        }

        public HttpStatusCode Status
        {
            get { return _status; }
        }

        public static void WriteHeaders(IHttpResponse response, IFubuFile file)
        {
            var mimeType = MimeType.MimeTypeByFileName(file.Path);
            if (mimeType != null)
            {
                response.AppendHeader(HttpResponseHeaders.ContentType, mimeType.Value);
            }

            response.AppendHeader(HttpResponseHeaders.LastModified, file.LastModified().ToString("r"));
            response.AppendHeader(HttpResponseHeaders.ETag, file.Etag().Quoted());

        }

        public override void Write(IHttpResponse response)
        {
            WriteHeaders(response, _file);

            if (_status == HttpStatusCode.OK)
            {
                response.AppendHeader(HttpResponseHeaders.ContentLength, _file.Length().ToString());
            }

            response.WriteResponseCode(_status);
        }
    }
}