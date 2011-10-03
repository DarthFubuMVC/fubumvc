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

        public virtual IHttpOutputWriter Writer
        {
            get { return _writer; }
        }

        private void revertToNormalWriting()
        {
            _state = new NormalState(this);
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


        public virtual RecordedOutput Record(Action action)
        {
            var recordingState = new RecordingState();
            _state = recordingState;

            action();

            var recordedOutput = new RecordedOutput(recordingState.ContentType, recordingState.Content);

            revertToNormalWriting();

            return recordedOutput;
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

        public virtual void WriteResponseCode(HttpStatusCode status)
        {
            Writer.WriteResponseCode(status);
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
            private readonly OutputWriter _parent;

            public NormalState(OutputWriter parent)
            {
                _parent = parent;
            }


            public void Write(string contentType, string renderedOutput)
            {
                _parent.Writer.WriteContentType(contentType);
                _parent.Writer.Write(renderedOutput);
            }
        }
    }
}