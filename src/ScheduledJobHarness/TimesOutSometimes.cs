using System;
using System.Threading;
using FubuCore;
using FubuMVC.Core.ServiceBus.Polling;

namespace ScheduledJobHarness
{
    public class TimesOutSometimes : IJob
    {
        private readonly Random _random = new Random();

        public void Execute(CancellationToken cancellation)
        {
            var next = _random.Next(0, 100);
            if (next > 50)
            {
                Thread.Sleep(1.Minutes());
            }
        }
    }
}