using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.HtmlWriting.Columns;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class DiagnosticBehavior : IActionBehavior
    {
        private readonly IDebugDetector _detector;
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

        public IActionBehavior Inner { get; set; }

        public void Invoke()
        {
            Inner.Invoke();

            write();
        }

        public void InvokePartial()
        {
            Inner.InvokePartial();

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