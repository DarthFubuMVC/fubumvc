using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using FubuCore;

namespace FubuMVC.Core.Runtime
{
    public class OutputWriter : IOutputWriter
    {
        private readonly IHttpOutputWriter _writer;
        private readonly IFileSystem _fileSystem;
        private IOutputState _state;

        public OutputWriter(IHttpOutputWriter writer, IFileSystem fileSystem)
        {
            _writer = writer;
            _fileSystem = fileSystem;
            revertToNormalWriting();
        }

        private void revertToNormalWriting()
        {
            _state = new NormalState(_writer);
        }

        public void WriteFile(string contentType, string localFilePath, string displayName)
        {
            _writer.WriteContentType(contentType);



			if (displayName != null)
			{
				_writer.AppendHeader("Content-Disposition", "attachment; filename=\"" + displayName+"\"");
			}

            var fileLength = _fileSystem.FileSizeOf(localFilePath);

			_writer.AppendHeader("Content-Length", fileLength.ToString());
            _writer.WriteFile(localFilePath);

        }


        public RecordedOutput Record(Action action)
        {
            var recordingState = new RecordingState();
            _state = recordingState;

            action();

            var recordedOutput = new RecordedOutput(recordingState.ContentType, recordingState.Content);

            revertToNormalWriting();

            return recordedOutput;
        }

        public void Write(string contentType, string renderedOutput)
        {
            _state.Write(contentType, renderedOutput);
        }

        public void RedirectToUrl(string url)
        {
            _writer.Redirect(url);
        }

        public void AppendCookie(HttpCookie cookie)
        {
            _writer.AppendCookie(cookie);
        }

        public void WriteResponseCode(HttpStatusCode status)
        {
            _writer.WriteResponseCode(status);
        }


        interface IOutputState
        {
            void Write(string contentType, string renderedOutput);
        }

        class RecordingState : IOutputState
        {
            private readonly StringBuilder _builder = new StringBuilder();

            public string ContentType { get; private set; }

            public string Content
            {
                get { return _builder.ToString(); }
            }

            public void Write(string contentType, string renderedOutput)
            {
                ContentType = contentType;
                _builder.Append(renderedOutput);
            }
        }

        class NormalState : IOutputState
        {
            private readonly IHttpOutputWriter _writer;

            public NormalState(IHttpOutputWriter writer)
            {
                _writer = writer;
            }

            public void Write(string contentType, string renderedOutput)
            {
                _writer.WriteContentType(contentType);
                _writer.Write(renderedOutput);
            }
        }
    }
}