using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime.Logging;
using Cookie = FubuMVC.Core.Http.Cookies.Cookie;

namespace FubuMVC.Core.Runtime
{
    public interface IHaveContentType
    {
        string ContentType { get; }
    }

    public class OutputWriter : IOutputWriter
    {
        private readonly IHttpResponse _response;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly Stack<IOutputState> _outputStates = new Stack<IOutputState>(); 

        public OutputWriter(IHttpResponse response, IFileSystem fileSystem, ILogger logger)
        {
            _response = response;
            _fileSystem = fileSystem;
            _logger = logger;

            normalWriting();
        }

        public virtual IHttpResponse Response
        {
            get { return _response; }
        }

        IOutputState CurrentState
        {
            get { return _outputStates.Peek(); }
        }

        private void normalWriting()
        {
            var state = new NormalState(_response, _fileSystem);
            _outputStates.Push(state);
        }

        public void WriteFile(string contentType, string localFilePath, string displayName)
        {
            _logger.DebugMessage(() => new FileOutputReport{
                ContentType = contentType,
                DisplayName = displayName,
                LocalFilePath = localFilePath
            });

            CurrentState.WriteFile(contentType, localFilePath, displayName);
        }

        public virtual IRecordedOutput Record(Action action)
        {
            _logger.DebugMessage(() => new StartedRecordingOutput());

            var output = new RecordedOutput(_fileSystem);
            _outputStates.Push(output);

            try
            {
                action();
            }
            finally
            {
                _outputStates.Pop();
            
                _logger.DebugMessage(() => new FinishedRecordingOutput(output));
            }

            return output;
        }

        public void Replay(IRecordedOutput output)
        {
            _logger.DebugMessage(() => new ReplayRecordedOutput(output));

            // We're routing the replay thru IOutputWriter to 
            // make unit testing easier, I think it gives a cleaner
            // dependency graph, and it makes request tracing work.
            output.Replay(Response);
        }

        // Keep this virtual for testing
        public virtual void Flush()
        {
            _logger.Debug(() => "Flushed content to the Http output");

            CurrentState.Flush();
        }

        public virtual void Write(string contentType, string renderedOutput)
        {
            _logger.DebugMessage(() => new OutputReport(contentType, renderedOutput));

            CurrentState.Write(contentType, renderedOutput);
        }

        public void Write(string renderedOutput)
        {
            _logger.DebugMessage(() => new OutputReport(renderedOutput));

            CurrentState.Write(renderedOutput);
        }

        public virtual void RedirectToUrl(string url)
        {
            _logger.DebugMessage(() => new RedirectReport(url));

            Response.Redirect(url);
        }


        public virtual void AppendCookie(Cookie cookie)
        {
            _logger.DebugMessage(() => new WriteCookieReport(cookie));

            CurrentState.AppendHeader(HttpResponseHeaders.SetCookie, cookie.ToString());
        }

        public void AppendHeader(string key, string value)
        {
            _logger.DebugMessage(() => new SetHeaderValue(key, value));

            CurrentState.AppendHeader(key, value);
        }

        public void Write(string contentType, Action<Stream> output)
        {
            _logger.DebugMessage(() => new WriteToStreamReport(contentType));
            
            CurrentState.Write(contentType, output);
        }

        public void WriteResponseCode(HttpStatusCode status, string description = null)
        {
            _logger.DebugMessage(() => new HttpStatusReport{
                Description = description,
                Status = status
            });

            Response.WriteResponseCode(status, description);
        }

        public void Dispose()
        {
        }
    }
}