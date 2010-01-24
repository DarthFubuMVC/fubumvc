using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Diagnostics
{
    public class DiagnosticBehavior : IActionBehavior
    {
        private readonly IDebugReport _report;
        private readonly IDebugDetector _detector;
        private readonly IActionBehavior _inner;
        private readonly IUrlRegistry _urls;

        public DiagnosticBehavior(IDebugReport report, IDebugDetector detector, IActionBehavior inner, IUrlRegistry urls)
        {
            _report = report;
            _detector = detector;
            _inner = inner;
            _urls = urls;
        }

        public void Invoke()
        {
            _inner.Invoke();

            write();
        }

        private void write()
        {
            if (!_detector.IsDebugCall()) return;
            _report.MarkFinished();

            var debugWriter = new DebugWriter(_report, _urls);
            var outputWriter = new HttpResponseOutputWriter();

            outputWriter.Write(MimeType.Html.ToString(), debugWriter.Write().ToString());
        }

        public void InvokePartial()
        {
            _inner.InvokePartial();

            write();
        }
    }
}