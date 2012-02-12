using System;
using System.Collections.Generic;
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
        private readonly Stack<IOutputState> _outputStates = new Stack<IOutputState>(); 

        public OutputWriter(IHttpWriter writer, IFileSystem fileSystem)
        {
            _writer = writer;
            _fileSystem = fileSystem;

            normalWriting();
        }

        public virtual IHttpWriter Writer
        {
            get { return _writer; }
        }

        IOutputState CurrentState
        {
            get { return _outputStates.Peek(); }
        }

        private void normalWriting()
        {
            var state = new NormalState(_writer, _fileSystem);
            _outputStates.Push(state);
        }

        public virtual void WriteFile(string contentType, string localFilePath, string displayName)
        {
            CurrentState.WriteFile(contentType, localFilePath, displayName);
        }

        public virtual IRecordedOutput Record(Action action)
        {
            var output = new RecordedOutput(_fileSystem);
            _outputStates.Push(output);

            try
            {
                action();
            }
            finally
            {
                _outputStates.Pop();
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
            CurrentState.Write(contentType, renderedOutput);
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
            CurrentState.AppendHeader(key, value);
        }

        public virtual void Write(string contentType, Action<Stream> output)
        {
            CurrentState.Write(contentType, output);
        }

        public virtual void WriteResponseCode(HttpStatusCode status)
        {
            Writer.WriteResponseCode(status);
        }
    }
}