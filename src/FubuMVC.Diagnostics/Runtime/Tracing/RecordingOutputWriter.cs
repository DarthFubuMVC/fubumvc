using System;
using System.Net;
using FubuCore;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Diagnostics.Runtime.Tracing
{
    // TODO -- eliminate most of the inherited code.  Do most of the work at the IHttpWriter level now
    public class RecordingOutputWriter : OutputWriter
    {
        private readonly IDebugDetector _detector;
        private readonly IDebugReport _report;

        public RecordingOutputWriter(IDebugReport report, IDebugDetector detector, IHttpWriter inner,
                                     IFileSystem fileSystem)
            : base(inner, fileSystem)
        {
            _report = report;
            _detector = detector;
        }

        public override IHttpWriter Writer
        {
            get
            {
                if (_detector.IsOutputWritingLatched())
                {
                    return new NulloHttpWriter();
                }

                return base.Writer;
            }
        }

        public override void WriteFile(string contentType, string localFilePath, string displayName)
        {
            throw new NotImplementedException();
            //_report.AddDetails(new FileOutputReport{
            //    ContentType = contentType,
            //    DisplayName = displayName,
            //    LocalFilePath = localFilePath
            //});

            base.WriteFile(contentType, localFilePath, displayName);
        }

        public override void Write(string contentType, string renderedOutput)
        {
            throw new NotImplementedException();
            //_report.AddDetails(new OutputReport{
            //    Contents = renderedOutput,
            //    ContentType = contentType
            //});

            base.Write(contentType, renderedOutput);
        }

        public override void RedirectToUrl(string url)
        {
            throw new NotImplementedException();
            //_report.AddDetails(new RedirectReport{
            //    Url = url
            //});

            base.RedirectToUrl(url);
        }


        public override void WriteResponseCode(HttpStatusCode status, string description = null)
        {
            throw new NotImplementedException();
            //_report.AddDetails(new HttpStatusReport{
            //    Status = status,
            //    Description = description
            //});
            base.WriteResponseCode(status, description);
        }

        public override IRecordedOutput Record(Action action)
        {
            var recordedOuput = base.Record(action);

            // TODO -- put this back!
            //_report.AddDetails(new OutputReport{
            //    Contents = recordedOuput.Content,
            //    ContentType = recordedOuput.ContentType
            //});


            return recordedOuput;
        }
    }
}