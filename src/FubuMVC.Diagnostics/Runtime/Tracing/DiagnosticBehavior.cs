using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Logging;
using FubuMVC.Core.Urls;

namespace FubuMVC.Diagnostics.Runtime.Tracing
{
    public class DiagnosticBehavior : WrappingBehavior
    {
        private readonly IDebugDetector _detector;
        private readonly IRequestTrace _trace;
        private readonly IOutputWriter _writer;
        private readonly IExceptionHandlingObserver _exceptionObserver;
        private readonly IUrlRegistry _urls;

        public DiagnosticBehavior(IRequestTrace trace, IDebugDetector detector, IOutputWriter writer, IExceptionHandlingObserver exceptionObserver, IUrlRegistry urls)
        {
            _trace = trace;
            _detector = detector;
            _writer = writer;
            _exceptionObserver = exceptionObserver;
            _urls = urls;
        }

        protected override void invoke(Action action)
        {
            _trace.Start();

            try
            {
                action();
            }
            catch (Exception ex)
            {
                _trace.MarkAsFailedRequest();

                if (!_exceptionObserver.WasObserved(ex))
                {
                    _trace.Log(new ExceptionReport("Request failed", ex));
                    _exceptionObserver.RecordHandled(ex);
                }

                throw;
            }
            finally
            {
                _trace.MarkFinished();

                if (_detector.IsDebugCall())
                {
                    _writer.RedirectToUrl(_urls.UrlFor(_trace.Current));
                }
            }
        }
    }
}