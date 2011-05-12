using System;
using System.Net;
using System.Web;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Diagnostics.Tracing
{
	public class RecordingOutputWriter : IOutputWriter
    {
        private readonly IDebugReport _report;
		private readonly IOutputWriter _inner;

        public RecordingOutputWriter(IDebugReport report, IOutputWriter inner)
        {
        	_report = report;
        	_inner = inner;
        }

		public void WriteFile(string contentType, string localFilePath, string displayName)
        {
            _report.AddDetails(new FileOutputReport()
            {
                ContentType = contentType,
                DisplayName = displayName,
                LocalFilePath = localFilePath
            });

			_inner.WriteFile(contentType, localFilePath, displayName);
        }

		public void Write(string contentType, string renderedOutput)
        {
            _report.AddDetails(new OutputReport()
            {
                Contents = renderedOutput,
                ContentType = contentType
            });

			_inner.Write(contentType, renderedOutput);
        }

		public void RedirectToUrl(string url)
        {
            _report.AddDetails(new RedirectReport()
            {
                Url = url
            });

			_inner.RedirectToUrl(url);
        }

		public void AppendCookie(HttpCookie cookie)
		{
			_inner.AppendCookie(cookie);
		}

		public void WriteResponseCode(HttpStatusCode status)
        {
			_report.AddDetails(new HttpStatusReport { Status = status });
			_inner.WriteResponseCode(status);
        }

	    public RecordedOutput Record(Action action)
	    {
	        throw new NotImplementedException();
	    }
    }
}