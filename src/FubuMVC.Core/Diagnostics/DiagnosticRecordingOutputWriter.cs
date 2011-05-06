using System;
using System.Net;
using System.Web;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Diagnostics
{
    public class DiagnosticRecordingOutputWriter : IOutputWriter
    {
        private readonly IDebugReport _report;

        public DiagnosticRecordingOutputWriter(IDebugReport report)
        {
            _report = report;
        }

        public void WriteFile(string contentType, string localFilePath, string displayName)
        {
            _report.AddDetails(new FileOutputReport()
            {
                ContentType = contentType,
                DisplayName = displayName,
                LocalFilePath = localFilePath
            });
        }

        public RecordedOuput Record(Action action)
        {
            //TODO: Recording Output Report?
            _report.AddDetails(new OutputReport()
                               {
                                   Contents = "recorded",
                                   ContentType = "recorded"
                               });
            return new RecordedOuput("","");
        }

        public void Write(string contentType, string renderedOutput)
        {
            _report.AddDetails(new OutputReport()
            {
                Contents = renderedOutput,
                ContentType = contentType
            });
        }

        public void RedirectToUrl(string url)
        {
            _report.AddDetails(new RedirectReport()
            {
                Url = url
            });
        }

        public void AppendCookie(HttpCookie cookie)
        {

        }

        public void WriteResponseCode(HttpStatusCode status)
        {

        }
    }
}