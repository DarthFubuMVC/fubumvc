using System;
using System.IO;
using System.Net;
using System.Web;
using FubuCore;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Runtime
{
    public class OutputWriter : IOutputWriter
    {
        private readonly IHttpWriter _writer;
        private readonly IFileSystem _fileSystem;
        private IOutputState _state;

        public OutputWriter(IHttpWriter writer, IFileSystem fileSystem)
        {
            _writer = writer;
            _fileSystem = fileSystem;
            revertToNormalWriting();
        }

        public virtual IHttpWriter Writer
        {
            get { return _writer; }
        }

        private void revertToNormalWriting()
        {
            _state = new NormalState(_writer);
        }

        public virtual void WriteFile(string contentType, string localFilePath, string displayName)
        {
            Writer.WriteContentType(contentType);

			if (displayName != null)
			{
				Writer.AppendHeader("Content-Disposition", "attachment; filename=\"" + displayName+"\"");
			}

            var fileLength = _fileSystem.FileSizeOf(localFilePath);

			Writer.AppendHeader("Content-Length", fileLength.ToString());
            Writer.WriteFile(localFilePath);
        }


        public virtual IRecordedOutput Record(Action action)
        {
            var output = new RecordedOutput();
            _state = output;

            try
            {
                action();
            }
            finally
            {
                revertToNormalWriting();
            }

            return output;
        }

        public void Replay(IRecordedOutput output)
        {
            // We're routing the replay thru IOutputWriter to 
            // make unit testing easier, I think it gives a cleaner
            // dependency graph, and it makes request tracing work.
            output.Replay(Writer);
        }

        public virtual void Write(string contentType, string renderedOutput)
        {
            _state.Write(contentType, renderedOutput);
        }

        public virtual void RedirectToUrl(string url)
        {
            Writer.Redirect(url);
        }

        public virtual void AppendCookie(HttpCookie cookie)
        {
            Writer.AppendCookie(cookie);
        }

        public void AppendHeader(string key, string value)
        {
            _state.AppendHeader(key, value);
        }

        public virtual void Write(string contentType, Action<Stream> output)
        {
            _state.Write(contentType, output);
        }

        public virtual void WriteResponseCode(HttpStatusCode status)
        {
            Writer.WriteResponseCode(status);
        }
    }

    public class NormalState : IOutputState
    {
        private readonly IHttpWriter _writer;

        public NormalState(IHttpWriter writer)
        {
            _writer = writer;
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
    }

    interface IOutputState
    {
        void Write(string contentType, string renderedOutput);
        void Write(string contentType, Action<Stream> action);

        void AppendHeader(string header, string value);
    }
}