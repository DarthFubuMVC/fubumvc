using System;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class BehaviorTracer : IActionBehavior
    {
        private readonly IDebugReport _report;
        private readonly IDebugDetector _debugDetector;

        public BehaviorTracer(IDebugReport report, IDebugDetector debugDetector)
        {
            _report = report;
            _debugDetector = debugDetector;
        }

        public IActionBehavior Inner { get; set; }

        public void Invoke()
        {
            invoke(() => Inner.Invoke());
        }

        public void InvokePartial()
        {
            invoke(() => Inner.InvokePartial());
        }

        private void invoke(Action action)
        {
            _report.StartBehavior(Inner);

            try
            {
                action();
            }
            catch (Exception ex)
            {
                _report.MarkException(ex);
                if (!_debugDetector.IsDebugCall())
                {
                    throw;
                }
            }

            _report.EndBehavior();
        }
    }
}