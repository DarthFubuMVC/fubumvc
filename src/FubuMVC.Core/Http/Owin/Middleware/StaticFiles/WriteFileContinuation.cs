﻿using System.Net;
using System.Threading.Tasks;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.Http.Owin.Middleware.StaticFiles
{
    public class WriteFileContinuation : WriterContinuation
    {
        private readonly IFubuFile _file;
        private readonly AssetSettings _settings;

        public WriteFileContinuation(IHttpResponse response, IFubuFile file, AssetSettings settings) : base(response, DoNext.Stop)
        {
            _file = file;
            _settings = settings;
        }

        public override Task Write(IHttpResponse response)
        {
            response.WriteFile(_file.Path);

            WriteFileHeadContinuation.WriteHeaders(response, _file);

            _settings.Headers.Each((key, source) => response.AppendHeader(key, source()));

            response.WriteResponseCode(HttpStatusCode.OK);

            return Task.CompletedTask;
        }

        public IFubuFile File => _file;

        public override string ToString()
        {
            return $"Write file: {_file}";
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