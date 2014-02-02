using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.OwinHost.Middleware.StaticFiles
{
    public class WriteFileHeadContinuation : WriterContinuation
    {
        private readonly IFubuFile _file;
        private readonly HttpStatusCode _status;

        public WriteFileHeadContinuation(IHttpWriter writer, IFubuFile file, HttpStatusCode status) : base(writer, DoNext.Stop)
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

        public static void WriteHeaders(IHttpWriter writer, IFubuFile file)
        {
            var mimeType = MimeType.MimeTypeByFileName(file.Path);
            if (mimeType != null)
            {
                writer.AppendHeader(HttpResponseHeaders.ContentType, mimeType.Value);
            }

            writer.AppendHeader(HttpResponseHeaders.LastModified, file.LastModified().ToString("r"));
            writer.AppendHeader(HttpResponseHeaders.ETag, file.Etag().Quoted());

        }

        public override void Write(IHttpWriter writer)
        {
            WriteHeaders(writer, _file);

            if (_status == HttpStatusCode.OK)
            {
                writer.AppendHeader(HttpResponseHeaders.ContentLength, _file.Length().ToString());
            }

            writer.WriteResponseCode(_status);
        }
    }
}