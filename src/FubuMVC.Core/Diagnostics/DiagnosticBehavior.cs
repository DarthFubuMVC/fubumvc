using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Diagnostics
{
    public class DiagnosticBehavior : IActionBehavior
    {
        private readonly IDebugDetector _detector;
        private IActionBehavior _inner;
        private readonly IDebugReport _report;
        private readonly IUrlRegistry _urls;

        public DiagnosticBehavior(IDebugReport report, IDebugDetector detector, IUrlRegistry urls,
                                  IRequestHistoryCache history)
        {
            _report = report;
            _detector = detector;
            _urls = urls;

            history.AddReport(report);
        }

        public void WrapBehavior(IActionBehavior inner)
        {
            _inner = inner;
        }

        public void Invoke()
        {
            _inner.Invoke();

            write();
        }

        public void InvokePartial()
        {
            _inner.InvokePartial();

            write();
        }

        private void write()
        {
            _report.MarkFinished();

            if (!_detector.IsDebugCall()) return;

            var debugWriter = new DebugWriter(_report, _urls);
            var outputWriter = new HttpResponseOutputWriter();

            outputWriter.Write(MimeType.Html.ToString(), debugWriter.Write().ToString());
        }
    }
}