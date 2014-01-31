using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost.Middleware.StaticFiles
{
    public class WriteStatusCodeContinuation : WriterContinuation
    {
        private readonly HttpStatusCode _status;
        private readonly string _reason;

        public WriteStatusCodeContinuation(IHttpWriter writer, HttpStatusCode status, string reason)
            : base(writer, DoNext.Stop)
        {
            _status = status;
            _reason = reason;
        }

        public override void Write(IHttpWriter writer)
        {
            writer.WriteResponseCode(_status, _reason);
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