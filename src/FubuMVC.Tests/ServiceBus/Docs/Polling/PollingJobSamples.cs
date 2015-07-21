using System;
using System.Threading;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;

namespace FubuTransportation.Testing.Docs.Polling
{
    // SAMPLE: PollingJobSamples
    public class PollingJobSampleTransportRegistry : FubuTransportRegistry
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