using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.OwinHost.Middleware.StaticFiles
{


    public class StaticFileMiddleware : FubuMvcOwinMiddleware
    {
        private readonly IFubuApplicationFiles _files;

        public StaticFileMiddleware(Func<IDictionary<string, object>, Task> inner, IFubuApplicationFiles files) : base(inner)
        {
            _files = files;
        }

        public override MiddlewareContinuation Invoke(ICurrentHttpRequest request, IHttpWriter writer)
        {
            if (request.IsNotHttpMethod("GET", "HEAD")) return MiddlewareContinuation.Continue();

            var file = _files.Find(request.RelativeUrl());
            if (file == null) return MiddlewareContinuation.Continue();




            // TODO -- check if the file extension is protected.  Put it on OwinSettings.  If protected, write a 404.  Make sure that "*.config" is protected

            // TODO -- do if-match header, exact match on etag.  Return 412 if not an exact match

            // TODO -- do if-none-match header.  Return 304 if the etag matches, otherwise write.

            // TODO -- do if-modified-since, return 304 if not modified, otherwise write

            // TODO -- do if-unmodified-since, return 412 if the file has been modified.  Looking for the same version as before.

            // TODO -- just write the file.

            throw new NotImplementedException();
        }

        public virtual MiddlewareContinuation WriteFile(IHttpWriter writer, IFubuFile file)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class WriterContinuation : MiddlewareContinuation
    {
        private readonly DoNext _doNext;

        protected WriterContinuation(IHttpWriter writer, DoNext doNext)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            DoNext = doNext;

            Action = () => Write(writer);
        }

        public abstract void Write(IHttpWriter writer);
    }

    public class WriteFileContinuation : WriterContinuation
    {
        private readonly IHttpWriter _writer;
        private readonly IFubuFile _file;

        public WriteFileContinuation(IHttpWriter writer, IFubuFile file) : base(writer, DoNext.Stop)
        {
        }

        public override void Write(IHttpWriter writer)
        {
            // TODO -- write the file w/ the right headers
            throw new NotImplementedException();
        }

        public IHttpWriter Writer
        {
            get { return _writer; }
        }

        public IFubuFile File
        {
            get { return _file; }
        }

        public override string ToString()
        {
            return string.Format("Write file: {0}", _file);
        }

        protected bool Equals(WriteFileContinuation other)
        {
            return Equals(_file, other._file);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WriteFileContinuation) obj);
        }

        public override int GetHashCode()
        {
            return (_file != null ? _file.GetHashCode() : 0);
        }
    }

    public class WriteStatusCodeContinuation : WriterContinuation
    {
        private readonly HttpStatusCode _code;
        private readonly string _reason;

        public WriteStatusCodeContinuation(IHttpWriter writer, HttpStatusCode code, string reason)
            : base(writer, DoNext.Stop)
        {
        }

        public override void Write(IHttpWriter writer)
        {
            // TODO write the status code and the reason
            throw new NotImplementedException();
        }

        public HttpStatusCode Code
        {
            get { return _code; }
        }

        public string Reason
        {
            get { return _reason; }
        }

        protected bool Equals(WriteStatusCodeContinuation other)
        {
            return _code == other._code && string.Equals(_reason, other._reason);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WriteStatusCodeContinuation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) _code*397) ^ (_reason != null ? _reason.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Stopping with Code: {0}, Reason: {1}", _code, _reason);
        }
    }
}