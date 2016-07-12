using System;
using System.IO;
using System.Threading.Tasks;
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

        public Task Write(string contentType, string renderedOutput)
        {
            _response.WriteContentType(contentType);
            return _response.Write(renderedOutput);
        }

        public Task Write(string contentType, Func<Stream, Task> action)
        {
            _response.WriteContentType(contentType);
            return _response.Write((Func<Stream, Task>) action);
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

        public Task Write(string renderedOutput)
        {
            return _response.Write(renderedOutput);
        }
    }
}