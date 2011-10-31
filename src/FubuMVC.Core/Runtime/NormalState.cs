using System;
using System.IO;
using FubuCore;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Runtime
{
    public class NormalState : IOutputState
    {
        private readonly IHttpWriter _writer;
        private readonly IFileSystem _fileSystem;

        public NormalState(IHttpWriter writer, IFileSystem fileSystem)
        {
            _writer = writer;
            _fileSystem = fileSystem;
        }

        public void Write(string contentType, string renderedOutput)
        {
            _writer.WriteContentType(contentType);
            _writer.Write(renderedOutput);
        }

        public void Write(string contentType, Action<Stream> action)
        {
            _writer.WriteContentType(contentType);
            _writer.Write(action);
        }

        public void AppendHeader(string header, string value)
        {
            _writer.AppendHeader(header, value);
        }

        public void WriteFile(string contentType, string localFilePath, string displayName)
        {
            var record = WriteFileRecord.Create(_fileSystem, localFilePath, contentType, displayName);
            record.Replay(_writer);
        }
    }
}