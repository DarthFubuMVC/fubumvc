using System;
using System.IO;
using FubuCore;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Runtime
{
    public class NormalState : IOutputState
    {
        private readonly IHttpResponse _response;
        private readonly IFileSystem _fileSystem;

        public NormalState(IHttpResponse response, IFileSystem fileSystem)
        {
            _response = response;
            _fileSystem = fileSystem;
        }

        public void Write(string contentType, string renderedOutput)
        {
            _response.WriteContentType(contentType);
            _response.Write(renderedOutput);
        }

        public void Write(string contentType, Action<Stream> action)
        {
            _response.WriteContentType(contentType);
            _response.Write(action);
        }

        public void AppendHeader(string header, string value)
        {
            _response.AppendHeader(header, value);
        }

        public void WriteFile(string contentType, string localFilePath, string displayName)
        {
            var record = WriteFileRecord.Create(_fileSystem, localFilePath, contentType, displayName);
            record.Replay(_response);
        }

        public void Flush()
        {
            _response.Flush();
        }

        public void Write(string renderedOutput)
        {
            _response.Write(renderedOutput);
        }
    }
}