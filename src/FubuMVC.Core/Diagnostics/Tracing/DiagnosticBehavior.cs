using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.HtmlWriting.Columns;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class DiagnosticBehavior : IActionBehavior
    {
        private readonly IDebugDetector _detector;
        private IDebugReport _report;
        private readonly IUrlRegistry _urls;
        private readonly IRequestHistoryCache _history;

        public DiagnosticBehavior(IDebugDetector detector, IUrlRegistry urls,
                                  IRequestHistoryCache history)
        {
            _detector = detector;
            _urls = urls;
            _history = history;
        }
        
        public IDebugReport Report
        {
            get { return _report; }
            set
            {
                if((_report = value) != null)
                {
                    _history.AddReport(value);
                }
            }
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