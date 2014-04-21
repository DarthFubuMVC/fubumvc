using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Diagnostics.Runtime.Tracing
{
    public class BehaviorTracer : WrappingBehavior
    {
        readonly BehaviorCorrelation _correlation;
        private readonly IRequestTrace _trace;
        readonly IExceptionHandlingObserver _exceptionObserver;

        public BehaviorTracer(BehaviorCorrelation correlation, IRequestTrace trace, IExceptionHandlingObserver exceptionObserver)
        {
            _correlation = correlation;
            _trace = trace;
            _exceptionObserver = exceptionObserver;
        }

        protected override void invoke(Action action)
        {
            _trace.Log(new BehaviorStart(_correlation));

            try
            {
                action();

                _trace.Log(new BehaviorFinish(_correlation));
            }
            catch (Exception ex)
            {
                if (!_exceptionObserver.WasObserved(ex))
                {
                    var log = new BehaviorFinish(_correlation);
                    log.LogException(ex);
                    _trace.Log(log);
                }

                throw;
            }
        }
    }
}