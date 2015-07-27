using System;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Scheduling;

namespace FubuMVC.Tests.ServiceBus.Docs.Transports
{
    // SAMPLE: LQMultipleChannelsSample
    public class LQSettings
    {
        public LQSettings()
        {
            BulkFile = new Uri("lq.tcp://localhost:2200/bulk_file_processing");
            RealTime = new Uri("lq.tcp://localhost:2200/real_time_message_processing");
        }

        public Uri BulkFile { get; set; }
        public Uri RealTime { get; set; }
    }

    public class MultipleChannelsLQTransport : FubuTransportRegistry<LQSettings>
    {
        public MultipleChannelsLQTransport()
        {
            Channel(x => x.BulkFile)
                .ReadIncoming(); //Default is 1 task

            Channel(x => x.RealTime)
                .ReadIncoming(new TaskScheduler(6));
        }
    }
    // ENDSAMPLE
}