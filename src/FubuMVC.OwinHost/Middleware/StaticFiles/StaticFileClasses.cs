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

            var ifMatch = request.IfMatch().EtagMatches(file.Etag());
            if (ifMatch == EtagMatch.No)
            {
                return new WriteStatusCodeContinuation(writer, HttpStatusCode.PreconditionFailed, "If-Match test failed");
            }

            var ifNoneMatch = request.IfNoneMatch().EtagMatches(file.Etag());
            if (ifNoneMatch == EtagMatch.Yes)
            {
                return new WriteFileHeadContinuation(writer, file, HttpStatusCode.NotModified);
            }
            
            if (ifNoneMatch == EtagMatch.No)
            {
                return new WriteFileContinuation(writer, file);
            }

            var ifModifiedSince = request.IfModifiedSince();
            if (ifModifiedSince.HasValue && file.LastModified() <= ifModifiedSince.Value.ToUniversalTime())
            {
                return new WriteFileHeadContinuation(writer, file, HttpStatusCode.NotModified);
            }

            var ifUnModifiedSince = request.IfUnModifiedSince();
            if (ifUnModifiedSince.HasValue && file.LastModified() > ifUnModifiedSince.Value)
            {
                return new WriteStatusCodeContinuation(writer, HttpStatusCode.PreconditionFailed, "File has been modified");
            }


            return new WriteFileContinuation(writer, file);
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


    public class WriteStatusCodeContinuation : WriterContinuation
    {
        private readonly HttpStatusCode _status;
        private readonly string _reason;

        public WriteStatusCodeContinuation(IHttpWriter writer, HttpStatusCode status, string reason)
            : base(writer, DoNext.Stop)
        {
            _status = status;
        }

        public override void Write(IHttpWriter writer)
        {

            // TODO write the status code and the reason
            throw new NotImplementedException();
        }



        public HttpStatusCode Status
        {
            get { return _status; }
        }

        public string Reason
        {
            get { return _reason; }
        }

        protected bool Equals(WriteStatusCodeContinuation other)
        {
            return _status == other._status && string.Equals(_reason, other._reason);
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
                return ((int) _status*397) ^ (_reason != null ? _reason.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Stopping with Code: {0}, Reason: {1}", _status, _reason);
        }
    }
}