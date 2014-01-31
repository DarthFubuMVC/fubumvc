using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;

namespace FubuMVC.OwinHost.Middleware.StaticFiles
{


    public class StaticFileMiddleware : FubuMvcOwinMiddleware
    {
        private readonly IFubuApplicationFiles _files;
        private readonly OwinSettings _settings;

        public StaticFileMiddleware(Func<IDictionary<string, object>, Task> inner, IFubuApplicationFiles files, OwinSettings settings) : base(inner)
        {
            _files = files;
            _settings = settings;
        }

        public override MiddlewareContinuation Invoke(ICurrentHttpRequest request, IHttpWriter writer)
        {
            if (request.IsNotHttpMethod("GET", "HEAD")) return MiddlewareContinuation.Continue();

            var file = _files.Find(request.RelativeUrl());
            if (file == null) return MiddlewareContinuation.Continue();

            if (_settings.DetermineStaticFileRights(file) != AuthorizationRight.Allow)
            {
                return MiddlewareContinuation.Continue();
            }


            if (request.IsHead())
            {
                return new WriteFileHeadContinuation(writer, file, HttpStatusCode.OK);
            }

            // TODO -- do if-match header, exact match on etag.  Return 412 if not an exact match

            // TODO -- do if-none-match header.  Return 304 if the etag matches, otherwise write.

            // TODO -- do if-modified-since, return 304 if not modified, otherwise write

            // TODO -- do if-unmodified-since, return 412 if the file has been modified.  Looking for the same version as before.

            // TODO -- just write the file.

            return new WriteFileContinuation(writer, file);
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
        private readonly IFubuFile _file;

        public WriteFileContinuation(IHttpWriter writer, IFubuFile file) : base(writer, DoNext.Stop)
        {
            _file = file;
        }

        public override void Write(IHttpWriter writer)
        {
            // content-type
            // content-length
            // etag
            // last modified

            // TODO -- write the file w/ the right headers
            throw new NotImplementedException();
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

        public override void Write(IHttpWriter writer)
        {
//            //if (!string.IsNullOrEmpty(_contentType))
//            {
//                _response.ContentType = _contentType;
//            }
//            _response.Headers.Set(Constants.LastModified, _lastModifiedString);
//            _response.ETag = _etagQuoted;
            throw new NotImplementedException();
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