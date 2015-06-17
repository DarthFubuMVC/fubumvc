using System;
using FubuCore;
using FubuCore.Descriptions;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Invocation;

namespace FubuTransportation.ErrorHandling
{
    public class DelayedRetryContinuation : IContinuation,DescribesItself
    {
        private readonly TimeSpan _delay;

        public DelayedRetryContinuation(TimeSpan delay)
        {
            _delay = delay;
        }

        public void Execute(Envelope envelope, ContinuationContext context)
        {
            envelope.Callback.MoveToDelayedUntil(context.SystemTime.UtcNow().Add(_delay));
        }

        public TimeSpan Delay
        {
            get { return _delay; }
        }

        public void Describe(Description description)
        {
            description.Title = "Retry in {0} seconds".ToFormat(_delay.TotalSeconds);
        }
    }
}