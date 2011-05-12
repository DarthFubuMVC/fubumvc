using System;
using System.Net;
using System.Web;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Diagnostics
{
    public class RecordingOutputWriter : IOutputWriter
    {
        private readonly IDebugReport _report;
        private IOutputWriter _inner;

        public RecordingOutputWriter(IDebugDetector detector, IDebugReport report)
        {
            _report = report;
            _inner = detector.IsDebugCall() ? (IOutputWriter)new NulloOutputWriter() : new HttpResponseOutputWriter();
        }

        // Open for testing
        public IOutputWriter Inner
        {
            get { return _inner; }
            set { _inner = value; }
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

        public RecordedOutput Record(Action action)
        {
            var recordedOuput = _inner.Record(action);

            _report.AddDetails(new OutputReport()
            {
                Contents = recordedOuput.Content,
                ContentType = recordedOuput.RecordedContentType
            });

            
            return recordedOuput;
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
            // TODO -- trace
            _inner.AppendCookie(cookie);
        }

        public void WriteResponseCode(HttpStatusCode status)
        {
            _report.AddDetails(new HttpStatusReport{
                Status = status
            });

            _inner.WriteResponseCode(status);
        }

    }
}