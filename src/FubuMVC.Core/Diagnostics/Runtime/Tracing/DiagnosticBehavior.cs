using System;
using FubuCore.Logging;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Logging;

namespace FubuMVC.Core.Diagnostics.Runtime.Tracing
{
    public class DiagnosticBehavior : WrappingBehavior
    {
        private readonly IRequestTrace _trace;
        private readonly IExceptionHandlingObserver _exceptionObserver;

        public DiagnosticBehavior(IRequestTrace trace, IExceptionHandlingObserver exceptionObserver)
        {
            _trace = trace;
            _exceptionObserver = exceptionObserver;
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
            }
        }
    }
}