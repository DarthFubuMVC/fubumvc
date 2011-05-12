using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace FubuMVC.Core.Runtime
{
    public class HttpResponseOutputWriter : IOutputWriter
    {
        private IOutputState _state = new NormalState();

        public void WriteFile(string contentType, string localFilePath, string displayName)
        {
            response.ContentType = contentType;

			if (displayName != null)
			{
				response.AppendHeader("Content-Disposition", "attachment; filename=\"" + displayName+"\"");
			}

			var fileInfo = new FileInfo(localFilePath);
			response.AppendHeader("Content-Length", fileInfo.Length.ToString());

            response.WriteFile(localFilePath);

        }


        public RecordedOutput Record(Action action)
        {
            _state = new RecordingState();

            action();

            var recordedOutput = new RecordedOutput(_state.RecordedContentType, _state.RecordedContent);

            _state = new NormalState();

            return recordedOutput;
        }

        private HttpResponse response
        {
            get { return HttpContext.Current.Response; }
        }

        public void Write(string contentType, string renderedOutput)
        {
            _state.Write(contentType, renderedOutput);
        }

        public void RedirectToUrl(string url)
        {
            response.Redirect(url, false);
        }

        public void AppendCookie(HttpCookie cookie)
        {
            response.AppendCookie(cookie);
        }

        public void WriteResponseCode(HttpStatusCode status)
        {
            response.StatusCode = (int)status;
        }


        interface IOutputState
        {
            void Write(string contentType, string renderedOutput);
            string RecordedContent { get; }
            string RecordedContentType { get; }
        }

        class RecordingState : IOutputState
        {
            private StringBuilder _builder = new StringBuilder();

            public string RecordedContentType { get; private set; }

            public string RecordedContent
            {
                get { return _builder.ToString(); }
            }

            public void Write(string contentType, string renderedOutput)
            {
                RecordedContentType = contentType;
                _builder.Append(renderedOutput);
            }
        }
        class NormalState : IOutputState
        {
            public void Write(string contentType, string renderedOutput)
            {
                response.ContentType = contentType;
                response.Write(renderedOutput);
            }

            public string RecordedContent
            {
                get { throw new NotImplementedException(); }
            }
            public string RecordedContentType { get { return response.ContentType; } }

            private HttpResponse response
            {
                get { return HttpContext.Current.Response; }
            }
        }
    }
}