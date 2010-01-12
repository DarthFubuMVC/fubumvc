using System;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Diagnostics
{
    public class RecordingOutputWriter : IOutputWriter
    {
        private readonly IDebugReport _report;

        public RecordingOutputWriter(IDebugReport report)
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
    }
}