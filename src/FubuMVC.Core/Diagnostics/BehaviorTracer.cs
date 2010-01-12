using System;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Diagnostics
{
    public class BehaviorTracer : IActionBehavior
    {
        private readonly IActionBehavior _inner;
        private readonly IDebugReport _report;
        private readonly IDebugDetector _debugDetector;

        public BehaviorTracer(IDebugReport report, IDebugDetector debugDetector, IActionBehavior inner)
        {
            _report = report;
            _debugDetector = debugDetector;
            _inner = inner;
        }

        public void Invoke()
        {
            invoke(() => _inner.Invoke());
        }

        public void InvokePartial()
        {
            invoke(() => _inner.InvokePartial());
        }

        private void invoke(Action action)
        {
            _report.StartBehavior(_inner);

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