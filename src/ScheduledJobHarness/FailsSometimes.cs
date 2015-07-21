using System;
using System.Threading;
using FubuMVC.Core.ServiceBus.Polling;

namespace ScheduledJobHarness
{
    public class FailsSometimes : IJob
    {
        private readonly Random _random = new Random();

        public void Execute(CancellationToken cancellation)
        {
            var next = _random.Next(0, 100);
            if (next > 50)
            {
                throw new DivideByZeroException("I fail sometimes");
            }
        }
    }
}