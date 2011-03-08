using System;
using System.Net;
using System.Web;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Diagnostics
{
    public class DebuggingOutputWriter : IOutputWriter
    {
        private readonly IOutputWriter _inner;

        public DebuggingOutputWriter(IDebugDetector detector, IDebugReport report)
        {
            _inner = detector.IsDebugCall() ? (IOutputWriter) new RecordingOutputWriter(report) : new HttpResponseOutputWriter();
        }

        public void WriteFile(string contentType, string localFilePath, string displayName)
        {
            _inner.WriteFile(contentType, localFilePath, displayName);
        }

        public void Write(string contentType, string renderedOutput)
        {
            _inner.Write(contentType, renderedOutput);
        }

        public void RedirectToUrl(string url)
        {
            _inner.RedirectToUrl(url);
        }

    	public void AppendCookie(HttpCookie cookie)
    	{
			_inner.AppendCookie(cookie);
    	}

    	public void WriteResponseCode(HttpStatusCode status)
        {
            _inner.WriteResponseCode(status);
        }

        public IOutputWriter Inner { get { return _inner; } }
    }
}