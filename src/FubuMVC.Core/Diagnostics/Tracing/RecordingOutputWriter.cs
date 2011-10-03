using System;
using System.Net;
using System.Web;
using FubuCore;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class RecordingOutputWriter : OutputWriter
    {
        private readonly IDebugReport _report;
        private readonly IDebugDetector _detector;

        public RecordingOutputWriter(IDebugReport report, IDebugDetector detector, IHttpOutputWriter inner, IFileSystem fileSystem)
            : base(inner, fileSystem)
        {
            _report = report;
            _detector = detector;
        }

        public override IHttpOutputWriter Writer
        {
            get
            {
                if (_detector.IsOutputWritingLatched())
                {
                    return new NulloHttpOutputWriter();
                }

                return base.Writer;
            }
        }

        public override void WriteFile(string contentType, string localFilePath, string displayName)
        {
            _report.AddDetails(new FileOutputReport{
                ContentType = contentType,
                DisplayName = displayName,
                LocalFilePath = localFilePath
            });

            base.WriteFile(contentType, localFilePath, displayName);
        }

        public override void Write(string contentType, string renderedOutput)
        {
            _report.AddDetails(new OutputReport{
                Contents = renderedOutput,
                ContentType = contentType
            });

            base.Write(contentType, renderedOutput);
        }

        public override void RedirectToUrl(string url)
        {
            _report.AddDetails(new RedirectReport{
                Url = url
            });

            base.RedirectToUrl(url);
        }

        
        public override void WriteResponseCode(HttpStatusCode status)
        {
            _report.AddDetails(new HttpStatusReport{
                Status = status
            });
            base.WriteResponseCode(status);
        }

        public override RecordedOutput Record(Action action)
        {
            var recordedOuput = base.Record(action);

            _report.AddDetails(new OutputReport
            {
                Contents = recordedOuput.Content,
                ContentType = recordedOuput.ContentType
            });


            return recordedOuput;
        }
    }
}