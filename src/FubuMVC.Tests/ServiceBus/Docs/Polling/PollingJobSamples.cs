using System;
using System.Threading;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;

namespace FubuMVC.Tests.ServiceBus.Docs.Polling
{
    // SAMPLE: PollingJobSamples
    public class PollingJobSampleTransportRegistry : FubuRegistry
    {
        public PollingJobSampleTransportRegistry()
        {
            Polling.RunJob<PomodoroJob>().ScheduledAtInterval<PomodoroSettings>(x => x.Interval);
        }
    }

    public class PomodoroJob : IJob
    {
        //Again, similar to message handlers this method is executed in a behavior chain
        public void Execute(CancellationToken cancellation)
        {
            //Lock doors and shutdown all distractions, time to get work done
        }
    }

    public class PomodoroSettings
    {
        public double Interval
        {
            get
            {
                return TimeSpan.FromMinutes(25).TotalMilliseconds;
            }
        }
    }
    // ENDSAMPLE
}