using System;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.OwinHost.Middleware.StaticFiles
{
    using SendFileFunc = Func<string, long, long?, CancellationToken, Task>;

    public class WriteFileContinuation : WriterContinuation
    {
        private readonly IFubuFile _file;

        public WriteFileContinuation(IHttpWriter writer, IFubuFile file) : base(writer, DoNext.Stop)
        {
            _file = file;
        }

        public override void Write(IHttpWriter writer)
        {
            writer.WriteFile(_file.Path);

            WriteFileHeadContinuation.WriteHeaders(writer, _file);
            writer.WriteResponseCode(HttpStatusCode.OK);
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
}