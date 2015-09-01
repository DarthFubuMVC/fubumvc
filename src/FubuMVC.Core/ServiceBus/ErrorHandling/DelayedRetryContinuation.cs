using System;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.ErrorHandling
{
    public class DelayedRetryContinuation : IContinuation,DescribesItself
    {
        private readonly TimeSpan _delay;

        public DelayedRetryContinuation(TimeSpan delay)
        {
            _delay = delay;
        }

        public void Execute(Envelope envelope, IEnvelopeContext context)
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