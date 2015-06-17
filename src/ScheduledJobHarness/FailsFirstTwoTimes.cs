using System;
using System.Threading;
using FubuTransportation.Polling;
using FubuTransportation.Runtime;

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