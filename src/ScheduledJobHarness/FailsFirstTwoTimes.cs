using System;
using System.Threading;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Runtime;

namespace ScheduledJobHarness
{
    public class FailsFirstTwoTimes : IJob
    {
        private readonly Envelope _envelope;

        public FailsFirstTwoTimes(Envelope envelope)
        {
            _envelope = envelope;
        }

        public void Execute(CancellationToken cancellation)
        {
            if (_envelope.Attempts < 3)
            {
                throw new DivideByZeroException("I fail on the first and second attempts");
            }
        }
    }
}